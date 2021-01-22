using Microsoft.Test.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WinAppDriver.Input;

namespace WinAppDriver.CommandHandlers.Helpers
{
    public class ActionsHelper
    {
        public class KeyAction
        {
            public char Char { get; set; }
            
            public string Type { get; set; }
        }

        public static bool TryGetPlainString(JArray actions, out string value, out KeyAction last)
        {
            last = null;
            value = null;
            try
            {
                var inputChars = actions
                    .Where(a => a["type"].Value<string>() == "keyDown" || a["type"].Value<string>() == "keyUp")
                    .Select(a =>
                    {
                        return new KeyAction
                        {
                            Char = a["value"].Value<char>(),
                            Type = a["type"].Value<string>()
                        };
                    });

                var pairs = inputChars
                    .Where((a, i) => i % 2 == 0)
                    .Zip(inputChars.Where((a, i) => i % 2 == 1), (first, second) => new[] { first, second });

                if (!pairs.All(p =>
                {
                    return p[0].Char == p[1].Char
                        && p[0].Type == "keyDown" && p[1].Type == "keyUp";
                }))
                {
                    return false;
                }

                var chars = pairs.Select(p => p[0].Char);
                if (chars.First() != '\0')
                {
                    return false;
                }

                chars = chars.Skip(1);
                if ((KeyboardActions.TryGetKey(chars.Last(), out Key key) && key == Key.Enter)
                    || (KeyboardActions.TryGetKey(chars.Last(), out key) && key == Key.Tab))
                {
                    last = pairs.Last()[0];
                    chars = chars.Take(chars.Count() - 1);
                }

                if (chars.All(a => IsPrintable(a)))
                {
                    value = new string(chars.ToArray());
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsPrintable(char c)
        {
            var nonRenderingCategories = new UnicodeCategory[] 
            {
                UnicodeCategory.Control,
                UnicodeCategory.OtherNotAssigned,
                UnicodeCategory.Surrogate
            };
            return char.IsWhiteSpace(c) || !nonRenderingCategories.Contains(char.GetUnicodeCategory(c));
        }
    }
}
