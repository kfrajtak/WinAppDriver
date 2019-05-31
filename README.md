# WinAppDriver
Selenium driver for WinForms applications

Any contributions are welcomed :)

This project is using 
- Windows Automation
- Nancy as the webserver
- XPathParser for parsing XPath expressions (https://github.com/quamotion/XPathParser)
- Selenium.WebDriver.3.141.0 to get the internals of web driver (commands for Nancy endpoints for example)
- windowsphonedriver (https://github.com/forcedotcom/windowsphonedriver) for the basic infrastructure (commands & command handlers)

Why another driver when there already is https://github.com/Microsoft/WinAppDriver? The app I'm testing is quite large with number of elements and that driver was timing out after 60 seconds on some XPath queries (see [this issue](https://github.com/Microsoft/WinAppDriver/issues/333)) and after putting everything together the query finished under 2 seconds.

Which Selenium commands are implemented?
- [X] acceptAlert 
  - default captions to locate the accpet button are `Ok` and `Yes`, additional captions can be added by setting `acceptAlertButtonCaptions` capability with a semicolon separated list of values
- [X] actions
- [X] clearElement
- [X] clickElement
- [X] close
- [ ] describeElement
- [X] dismissAlert 
  - default captions to locate the accpet button are `Cancel` and `No`, additional captions can be added by setting `dismissAlertButtonCaptions` capability with a semicolon separated list of values
- [ ] elementEquals
- [ ] executeAsyncScript
- [ ] executeScript
- [X] findChildElement
- [X] findChildElements
- [X] findElement
- [X] findElements
- [ ] get
- [X] getActiveElement
- [X] getAlertText
- [X] getCurrentWindowHandle
- [X] getElementAttribute
- [ ] getElementLocation
- [ ] getElementLocationOnceScrolledIntoView
- [X] getElementSize
- [X] getElementTagName
- [X] getElementText
- [ ] getElementValueOfCssProperty
- [ ] getSessionCapabilities
- [ ] getSessionList
- [X] getTitle
- [X] getWindowHandles
- [X] getWindowPosition
- [X] getWindowRect/getWindowSize
- [X] implicitlyWait
- [X] isElementDisplayed
- [X] isElementEnabled
- [X] isElementSelected
- [X] maximizeWindow
- [X] mouseClick
- [X] mouseDoubleClick
- [X] mouseDown
- [X] mouseMoveTo
- [X] mouseUp
- [X] newSession
- [X] quit
- [X] screenshot
- [X] sendKeysToActiveElement
- [X] sendKeysToElement
- [ ] setAlertValue
- [ ] setTimeout
- [ ] setWindowPosition
- [X] setWindowSize
- [ ] status
- [ ] submitElement
- [X] switchToWindow
- [ ] touchDoubleTap
- [ ] touchDown
- [ ] touchFlick
- [ ] touchLongPress
- [ ] touchMove
- [ ] touchScroll
- [ ] touchSingleTap
- [ ] touchUp
- [ ] uploadFile

Unsupported Selenium commands
- addCookie
- deleteAllCookies
- deleteCookie
- getCookies
- getCurrentUrl
- getOrientation
- getPageSource
- goBack
- goForward
- refresh
- setOrientation
- setScriptTimeout
- switchToFrame

XPath support:
- [ ] axes 
  - [ ] Ancestor
  - [ ] AncestorOrSelf
  - [X] Attribute
  - [X] Child        
  - [X] Descendant    
  - [X] DescendantOrSelf
  - [ ] Following
  - [ ] FollowingSibling
  - [ ] Namespace
  - [X] Parent
  - [ ] Preceding
  - [ ] PrecedingSibling
  - [X] Self
  - [X] Root
- [X] predicates
  - [X] position predicate `//node[3]`
  - [X] attribute predicate `//node[@attribute = 'X']`
- [ ] operators
  - [X] Or
  - [X] And
  - [X] Eq 
  - [X] Ne
  - [X] Lt 
  - [X] Le 
  - [X] Gt 
  - [X] Ge
  - [X] Plus 
  - [X] Minus 
  - [X] Multiply 
  - [X] Divide 
  - [X] Modulo
  - [X] UnaryMinus
  - [ ] Union
 - [ ] functions (https://developer.mozilla.org/en-US/docs/Web/XPath/Functions)
  - [ ] boolean()
  - [ ] ceiling()
  - [ ] choose()
  - [ ] concat()
  - [X] contains()
  - [ ] count()
  - [ ] current() XSLT-specific
  - [ ] document() XSLT-specific
  - [ ] element-available()
  - [ ] false()
  - [ ] floor()
  - [ ] format-number() XSLT-specific
  - [ ] function-available()
  - [ ] generate-id() XSLT-specific
  - [ ] id() (partially supported)
  - [ ] key() XSLT-specific
  - [ ] lang()
  - [ ] last()
  - [ ] local-name()
  - [ ] name()
  - [ ] namespace-uri()
  - [X] normalize-space()
  - [ ] not()
  - [ ] number()
  - [ ] position()
  - [ ] round()
  - [ ] starts-with()
  - [ ] string()
  - [ ] string-length()
  - [ ] substring()
  - [ ] substring-after()
  - [ ] substring-before()
  - [ ] sum()
  - [ ] system-property() XSLT-specific
  - [ ] translate()
  - [ ] true()
  - [ ] unparsed-entity-url() XSLT-specific (not supported)


## How to create session
Add reference to `Selenium.WebDriver` (https://www.nuget.org/packages/Selenium.WebDriver/) and you are ready to go.

The driver is currently not able to start the system under test. You have to set process name in capapabilities. The IP address is currently hardwired to `http://127.0.0.1:4444`.

```
public static RemoteWebDriver CreateSession()
{
  DesiredCapabilities desktopCapabilities = new DesiredCapabilities();
  desktopCapabilities.SetCapability("processName", "<name of the process>");
  return new RemoteWebDriver(
    new CommandExec(new Uri("http://127.0.0.1:4444"), 
    TimeSpan.FromSeconds(60)), 
    desktopCapabilities);
}
```

Recommended element location is using XPath expression (though with a limited expression support)
```
var webBrowser = session.FindElement(By.XPath("//Pane[@AutomationId='webBrowser']"));
```

Windows in the Win application are not like windows in browser. The windows (`ControlType.Window`) can be nested inside the control tree, for example in a `Tab` element. 

Window can be either located using XPath expression `var window = session.FindElement(By.XPath("/Window/Pane/Window[@AutomationId='WindowName']"));` or by switching to it `session.SwitchTo().Window("WindowName");`, in the first example you will get the element reference, in the other the internal context is switched to new window and elements cached during the following operations can be disposed when window is cloded `session.Close()`.

Note that element wrappers like `OpenQA.Selenium.Support.UI.SelectElement` do not work because internally `select` and `option` elements are expected.
