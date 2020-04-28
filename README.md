# WinAppDriver
Selenium driver for WinForms applications

Any contributions are welcomed :)

This project is using 
- Windows Automation
- Nancy as the webserver
- XPathParser for parsing XPath expressions (https://github.com/quamotion/XPathParser)
- Selenium.WebDriver.3.141.0 to get the internals of web driver (commands for Nancy endpoints for example)
- windowsphonedriver (https://github.com/forcedotcom/windowsphonedriver) for the basic infrastructure (commands & command handlers)

Why another driver, when there already is https://github.com/Microsoft/WinAppDriver? The app I'm testing is quite large with many elements, and that driver was timing out after 60 seconds on some XPath queries (see [this issue](https://github.com/Microsoft/WinAppDriver/issues/333)), and after putting everything together, the query finished under 2 seconds.

## Installation
Currently, there is no installer. Clone the repository and build the executable from the sources.

## Selenium
### Client driver version
Latest client driver version 3.141.0.0 must be used when calling the server.

### Which Selenium commands are implemented?
- [X] acceptAlert 
  - default captions to locate the accept button are `Ok` and `Yes`, additional captions can be added by setting `acceptAlertButtonCaptions` capability with a semicolon separated list of values
- [X] actions
- [X] clearElement
- [X] clickElement
- [X] close
- [ ] describeElement
- [X] dismissAlert 
  - default captions to locate the accept button are `Cancel` and `No`, additional captions can be added by setting `dismissAlertButtonCaptions` capability with a semicolon separated list of values
- [ ] elementEquals
- [ ] executeAsyncScript
- [ ] executeScript
- [X] findChildElement
- [X] findChildElements
- [X] findElement
- [X] findElements
- [X] getActiveElement
- [X] getAlertText
- [X] getCurrentWindowHandle
- [X] getElementAttribute
- [X] getElementLocation
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
- [X] setTimeout
- [X] setWindowPosition
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
- get
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
  - [X] starts-with()
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

## Appium Support
Only this Appium command is implemented
  - [X] closeApp

## How to create a session
Add a reference to `Selenium.WebDriver` v3.141.0 (https://www.nuget.org/packages/Selenium.WebDriver/3.141.0) and you are ready to go.

The driver can either start the system under test process or attach to a running process. Use capabilities to define the process to attach to. 

When no command-line argument is provided, the server will be launched at default IP address `http://127.0.0.1:4444`.

### Desired capabilities
Following capabilities are supported:

  - `mode` - `start` (default value) or `attach`    
  - `processId` - id of the process to attach to
  - `processName` - name of the process to attach to
  - `exePath` or `app` - path to the executable to start the process (arguments cannot be provided at the moment)
  - `mainWindowTitle` - regular expression to help the WinAppDriver narrow down the process to attach to 

### Creating session

```
public static RemoteWebDriver CreateSessionByStartingTheApplication()
{
  DesiredCapabilities desktopCapabilities = new DesiredCapabilities();
  desktopCapabilities.SetCapability("app", "<name of the program to start>");  
  // or "exePath" desktopCapabilities.SetCapability("exePath", "<path to the executable to start the process>");  
  // following capabilities should be provided for UWP applications like Calculator or Clocks & Alarms 
  // optional - to identify the process
  desktopCapabilities.SetCapability("processName", "<name of the process>"); 
  // optional - to identify the main window
  desktopCapabilities.SetCapability("mainWindowTitle", "<name of the process>");  
  return new RemoteWebDriver(
    new CommandExec(new Uri("http://127.0.0.1:4444"), 
    TimeSpan.FromSeconds(60)), 
    desktopCapabilities);
}
```

```
public static RemoteWebDriver CreateSessionByAttachingToRunningProcess()
{
  DesiredCapabilities desktopCapabilities = new DesiredCapabilities();
  desktopCapabilities.SetCapability("mode", "attach");
  // attach to process using process name
  desktopCapabilities.SetCapability("processName", "<name of the process to attach to>");
  // with (optional)
  desktopCapabilities.SetCapability("windowTitle", "<regular expression to narrow down the list of matching processes>");
  // or attach to process using process id
  desktopCapabilities.SetCapability("processId", "<id of the process to attach to>");
  
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

Element location mechanisms that are supported
- XPath
- Id, i.e. UI automation element automation id
- CSS class (limited)
  - tag name, for example `TextBox`
  - id, for example `#id`
- Class name, i.e. UI automation element class name
- Tag name
- Accessibility id, i.e. UI automation element automation id
- Name, i.e. UI automation element name

Windows in the Win application are not like windows in the browser. The windows (`ControlType.Window`) can be nested inside the control tree, for example in a `Tab` element. 

Window can be either located using XPath expression `var window = session.FindElement(By.XPath("/Window/Pane/Window[@AutomationId='WindowName']"));` or by switching to it `session.SwitchTo().Window("WindowName");`, in the first example you will get the element reference, in the other the internal context is switched to new window and elements cached during the following operations can be disposed when window is cloded `session.Close()`.

Note that element wrappers like `OpenQA.Selenium.Support.UI.SelectElement` do not work because internally `select` and `option` elements are expected.

## Note
There is quite an ugly [hack](https://github.com/kfrajtak/WinAppDriver/blob/master/WinAppDriver/CommandHandlers/FindChildElementCommandHandler.cs#L56) which bypasses the need to switch windows when searching for a child window using XPath expression. By default, the search starts at the root element of a window - the main window or the window the user switched to - and the search for a child window was failing since it was not a direct child of the current root. The fix starts the search for a window at the top level.
