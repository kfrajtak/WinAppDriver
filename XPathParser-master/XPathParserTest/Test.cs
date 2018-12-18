using System;
using System.Collections.Generic;
using System.Text;
using CodePlex.XPathParser;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml;

namespace XPathParserTest {
    class Test {
        static string[] correctTests = {
             // Expressions from http://www.w3.org/TR/xpath#location-paths
            @"book[last()]",
            @"child::para"                                                          ,
            @"child::*"                                                             ,
            @"child::text()"                                                        ,
            @"child::node()"                                                        ,
            @"attribute::name"                                                      ,
            @"attribute::*"                                                         ,
            @"descendant::para"                                                     ,
            @"ancestor::div"                                                        ,
            @"ancestor-or-self::div"                                                ,
            @"descendant-or-self::para"                                             ,
            @"self::para"                                                           ,
            @"child::chapter/descendant::para"                                      ,
            @"child::*/child::para"                                                 ,
            @"/"                                                                    ,
            @"/descendant::para"                                                    ,
            @"/descendant::olist/child::item"                                       ,
            @"child::para[position()=1]"                                            ,
            @"child::para[position()=last()]"                                       ,
            @"child::para[position()=last()-1]"                                     ,
            @"child::para[position()>1]"                                            ,
            @"following-sibling::chapter[position()=1]"                             ,
            @"preceding-sibling::chapter[position()=1]"                             ,
            @"/descendant::figure[position()=42]"                                   ,
            @"/child::doc/child::chapter[position()=5]/child::section[position()=2]",
            @"child::para[attribute::type=""warning""]"                             ,
            @"child::para[attribute::type='warning'][position()=5]"                 ,
            @"child::para[position()=5][attribute::type=""warning""]"               ,
            @"child::chapter[child::title='Introduction']"                          ,
            @"child::chapter[child::title]"                                         ,
            @"child::*[self::chapter or self::appendix]"                            ,
            @"child::*[self::chapter or self::appendix][position()=last()]"         ,
            @"//Group[@AutomationId='_MainForm_Toolbars_Dock_Area_Top']/MenuBar/MenuItem[5]"
        };

        static string[] errorTests = {
            @""     ,
            @"a b"  ,
            @"a["   ,
            @"]"    ,
            @"///"  ,
            @"fo("  ,
            @")"    ,
            @"a[']" ,
            @"b[""]",
            @"3e8"  ,
            @"child::*[self::chapter or self::appendix][position()=last()] child::*[self::chapter or self::appendix][position()=last()]",
        };

        static void Main(string[] args) {
            foreach (string testCase in correctTests) {
                RunTestString(testCase);
            }
            Console.WriteLine();

            foreach (string testCase in correctTests) {
                RunTestTree(testCase);
            }
            Console.WriteLine();

            RunTestX("textbox");
            RunTestX("//Group[@AutomationId='_MainForm_Toolbars_Dock_Area_Top']/MenuBar/MenuItem[5]");

            Console.WriteLine("------------- Error Cases -----------------");
            foreach (string testCase in errorTests) {
                RunTestTree(testCase);
            }
        }

        static void RunTestString(string xpathExpr) {
            Console.WriteLine("Original XPath: {0}", xpathExpr);
            try {
                var x = new XPathParser<string>().Parse(xpathExpr, new XPathStringBuilder());
                Console.WriteLine("Translated one: {0}", x);
            } catch (XPathParserException e) {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine();
        }

        static void RunTestX(string xpathExpr)
        {
            Console.WriteLine("Original XPath: {0}", xpathExpr);
            try
            {
                var x = new XPathParser<IControlWalkerElement>().Parse(xpathExpr, new ControlWalkerBuilder());
                Console.WriteLine("Translated one: {0}", x);
            }
            catch (XPathParserException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine();
        }

        static void RunTestTree(string xpathExpr) {
            Console.WriteLine("Original XPath: {0}", xpathExpr);
            try {
                XElement xe = new XPathParser<XElement>().Parse(xpathExpr, new XPathTreeBuilder());
                XmlWriterSettings ws = new XmlWriterSettings(); {
                    ws.Indent = true;
                    ws.OmitXmlDeclaration = true;
                }
                using (XmlWriter w = XmlWriter.Create(Console.Out, ws)) {
                    xe.WriteTo(w);
                }                
            } catch (XPathParserException e) {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}

