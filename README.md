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
- [ ] acceptAlert
- [X] actions
- [ ] addCookie
- [ ] clearElement
- [X] clickElement
- [X] close
- [ ] deleteAllCookies
- [ ] deleteCookie
- [ ] describeElement
- [ ] dismissAlert
- [ ] elementEquals
- [ ] executeAsyncScript
- [ ] executeScript
- [X] findChildElement
- [X] findChildElements
- [X] findElement
- [X] findElements
- [ ] get
- [ ] getActiveElement
- [ ] getAlertText
- [ ] getCookies
- [ ] getCurrentUrl
- [ ] getCurrentWindowHandle
- [ ] getElementAttribute
- [ ] getElementLocation
- [ ] getElementLocationOnceScrolledIntoView
- [ ] getElementSize
- [ ] getElementTagName
- [ ] getElementText
- [ ] getElementValueOfCssProperty
- [ ] getOrientation
- [ ] getPageSource
- [ ] getSessionCapabilities
- [ ] getSessionList
- [ ] getTitle
- [ ] getWindowHandles
- [ ] getWindowPosition
- [ ] getWindowSize
- [ ] goBack
- [ ] goForward
- [ ] implicitlyWait
- [ ] isElementDisplayed
- [ ] isElementEnabled
- [X] isElementSelected
- [ ] maximizeWindow
- [ ] mouseClick
- [ ] mouseDoubleClick
- [ ] mouseDown
- [ ] mouseMoveTo
- [ ] mouseUp
- [ ] newSession
- [X] quit
- [ ] refresh
- [ ] screenshot
- [X] sendKeysToActiveElement
- [X] sendKeysToElement
- [ ] setAlertValue
- [ ] setOrientation
- [ ] setScriptTimeout
- [ ] setTimeout
- [ ] setWindowPosition
- [ ] setWindowSize
- [ ] status
- [ ] submitElement
- [ ] switchToFrame
- [ ] switchToWindow
- [ ] touchDoubleTap
- [ ] touchDown
- [ ] touchFlick
- [ ] touchLongPress
- [ ] touchMove
- [ ] touchScroll
- [ ] touchSingleTap
- [ ] touchUp
- [ ] uploadFile

XPath supported:
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
  - [ ] Self
  - [X] Root
- [ ] predicates
  - [X] position predicate `//node[3]`
  - [X] attribute predicate `//node[@attribute = 'X']`
- [ ] operators
  - [ ] Or
  - [ ] And
  - [X] Eq 
  - [ ] Ne
  - [ ] Lt 
  - [ ] Le 
  - [ ] Gt 
  - [ ] Ge
  - [ ] Plus 
  - [ ] Minus 
  - [ ] Multiply 
  - [ ] Divide 
  - [ ] Modulo
  - [ ] UnaryMinus
  - [ ] Union
 - [ ] methods

## How to create session
The driver is currently not able to start the system under test. You have to set process name in capapabilities. The IP address is currently hardwired to `http://127.0.0.1:12345`.

```public static RemoteWebDriver CreateSession()
{
	DesiredCapabilities desktopCapabilities = new DesiredCapabilities();
	desktopCapabilities.SetCapability("processName", "<name of the process>");
	return new RemoteWebDriver(new CommandExec(new Uri("http://127.0.0.1:12345"), TimeSpan.FromSeconds(60)), desktopCapabilities);
}```
