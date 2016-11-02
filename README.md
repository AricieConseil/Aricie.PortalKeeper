# Aricie.PortalKeeper
Portal Keeper is a DNN module to build **agents**, with a sequential **rule engine** embedded in several server components, covering many aspects of a complex web application project.

##### _Need help with configuration, a custom rule, a dedicated agent or a new action provider? Want to share some feedback or contribute some improvements?_
##### _Please let [us](http://www.aricie.com) know and get the project going !_

## What's up with PortalKeeper
#### 06/22/2016 Release: [Version 4.3.3](http://ariciepkp.codeplex.com/releases/view/624803) with new _Sql Job Bot_, fixes and service agents improvements
#### 06/04/2016 AI with PKP was demonstrated in a [DNN Connect session](http://dnn-connect.org/events/2016/sessions/moduleId/736/conferenceId/2/sessionId/68/controller/Session/action/View), the video is available on [MS Channel 9](http://channel9.msdn.com/Events/Microsoft-Spain-Events/DNN-Connect/Artificial-Intelligence-in-DNN)
##### 04/12/2016 Release: [Version 4.3.2](https://ariciepkp.codeplex.com/releases/view/620875) with new _AI services (vol. 2)_, Markdown parser, asset manager editor adapter and serialization enhancements
##### 01/27/2016 Release: [Version 4.3.1](https://ariciepkp.codeplex.com/releases/view/619571) with new _AI services_, referrer spammer firewall rule, string processing and serialization enhancements
##### 10/15/2015 Release: [Version 4.3.0](https://ariciepkp.codeplex.com/releases/view/617926) with new _Application life cycle_ engine, critical changes logging and _recycle free development environment_
##### 10/08/2015 Release: [Version 4.2.3](https://ariciepkp.codeplex.com/releases/view/617815) with a dynamic Keepalive.aspx adapter and usability improvements 
##### 09/16/2015 Release: [Version 4.2.2](https://ariciepkp.codeplex.com/releases/view/617424) with Custom _Dynamic Types_, Spreadsheet _Restful CRUD service_, spreadsheet structured queries, partial xml signature.
##### 09/11/2015 Release: [Version 4.2.1](https://ariciepkp.codeplex.com/releases/view/617329) with _Streaming Proxy_, Response bandwidth Throttling, dynamic string transformations and regular expressions callbacks.
##### 08/26/2015 Release: [Version 4.2.0](https://ariciepkp.codeplex.com/releases/view/617000) with _Web API Rest services_ and _Virtual Custom Errors handler_ reintroduced.
##### 08/19/2015 News - _Joe Brinkman_ and _Will Strohl_ introduce **Aricie.PortalKeeper** during the [August, 2015 DNNHangout](https://youtu.be/xadBtuJYNwg).
##### 12/07/2014 Tutorials - An introduction [video tutorial](https://www.youtube.com/playlist?list=PL53OdxVme3uj5Zp28G_1VE5w4brZYMzY2) to the module through example: activation the Recaptcha Adapter and creation of an agent from scratch.

## What's inside Portal Keeper

Portal Keeper can be installed as a regular extension to a [DNN](http://www.dnnsoftware.com/) web application.
It runs 6 flavors of the same application engine, that make up the sections of the main configuration form.

## Application firewall

This firewall takes the form of a _hybrid Http Module_ where you can attach your rule condition evaluation and actions execution to the inner events of the Asp.Net and Page life cycles.

Default rules include:

* **Filter Html Response** : Change the response text before it is streamed to the client. A rich combination of global and local string transformations, html xpath queries,  token replace and [Markdown](https://en.wikipedia.org/wiki/Markdown) syntax parsing are demonstrated by default.
* **Cache Http Output** : Use a set of strategies to trigger and finely control ASP.Net response caching with great performance gains.
* **Create Content Delivery Network**: Dedicate one of your portal alias to serving stylesheets, scripts, images, and other resources, with or without an worldwide 3rd party CDN service. Intercept your Html output to set the corresponding Urls, and optimize CDN responses.
* **Protect from DDoS attacks** : Monitor incoming Http calls with rate caps in order to filter out rogue requests.
* **Protect from Bandwidth overuse with Throttler** : Monitor bandwidth usage and put a bandwidth cap on heavy users.
* **Control Critical Accounts access**: Define a restricted perimeter for administrator logins based on IPs, aliases, countries etc.
* **Block Referrer spammers**: compare the current request's referrer Host to a list of (8000+) spammer domains, and terminate the request if matched
* Detect and/or prevent **simultaneous connections** from the same user accounts
* **Mobile devices redirection**: That rule illustrate setting up a custom condition from accessing a global Dnn and Http context and evaluating custom regular expressions against it.
* **Super User backdoor** : Set up an easy autologin secret url under restricted client source conditions
* **Geographically limited registration** : Adjust your registering settings according to the detected client country.
* **Warn IE 6 users** :  Display a page or module message according to custom conditions. 
* **Display custom information** on demand, such as the server name in a web farm scenario, with a dedicated query string parameter.
* **Streaming Proxy filter** associated to the powerful proxy handler, that uses a Pipe stream to relay external Requests efficiently, bypassing same-origin policy issues when CORS headers are not available. That rule additionally filters html, js, css content to proxify inner links, but also processes Kml and Kmz files to proxify external resources and set visibility of inner geographic entities to true.
* **Clear cache on demand** by providing the key to be cleared as a request parameter
The FIrewall also contains custom components such as a Ban List with quick connection shut down action, which is leveraged by the DDoS Rule.


## Web bots farm

In that component, the engine takes the form of a farm of scheduled bots embedded within the native DNN task scheduler.
An instance of each bot can additionally be proposed for customization to regular DNN registered account.

Sample Bots include:

* **Keep Alive Pings** - Performs Http Requests to a series of Urls to keep the target websites up and monitor response times, and sends alert emails in case of unavailability.
* **Sql Job** - runs a stored procedure, converts, filters the results, and emails the portal admin with the resulting list of recently active users, injected into the message with loop tokens
* **Real Estate Monitoring** - Harvests classifieds, and updates a Google spreadsheets document with targeted results. 
* **Search Result Scraper**: Define a series a web searches to monitor, together with paging settings, scrap data from result pages through dedicated xpath queries and sub queries, dive deeper with detail http sub requests, perform custom processing in IronPython, and keep track of the resulting data in a dedicated Google spreadsheet with a custom identifying key.
* **Google nearby places** - Monitors custom Google queries and send alerts when new results are available. That bot also illustrates maintaining a pool of available open proxies to be used for subsequent Http requests.
* **Bitcoin trading** bots (soon) - those bots connect to Bitcoin exchange APIs and issue orders based on custom trading strategies. 
* **Reddit Commands** bot (soon) - This bot will connect to the Reddit API as a dedicated user, and monitor messages, posts or subreddits, scanning for custom commands, answering and performing actions accordingly. The goal is that each user bot can embed a personal instance of several of the famous bots operating on the platform.  

## Rest Services

That component let you define Rest Web services with an engine associated to each method. 
An earlier version was developed using the OpenRasta framework. The new version provide hooks to the web API framework and corresponding DNN Service Framework.
The engine allows for dynamic routes, controllers, actions, parameters, and attributes. Routes are also automatically discovered.

A collection of [Postman](https://www.getpostman.com) requests is also available [for download](https://www.getpostman.com/collections/e380c6c4df7092d47be5) to help testing the default services.

Sample Services include:

* **Crypto Service** - A service tied to hosted encryption parameters to perform on demand encryption, decryption, XML full or partial signature and Signature verification. it illustrates the use of custom parameters, custom routes, and custom attributes.

* **Spreadsheet Service** - A service wrapping Google spreadsheet services, providing a custom entity Restful CRUD with automatic column conversion. This service can serve as a template to build CRUD services.

* **DNN Utilities** - Controllers to expose DNN features and model to the client side, with a sample user current controller.

* **API** - Services to configure the platform's agents and settings.

* **Artificial Intelligence** - Collection of AI services exposing various domain engines with processing and configurable strategies
List of AI controllers:
  * **Problem solving**: graph search and tree search with Uninformed and informed strategies (Breadth first, iterative depth first, recursive A star, greedy best first, hill climbing, simulated annealing etc.)
  * **Constraint satisfaction problems**: static or dynamic definition, Improved backtracking with AC3, Minimum Remaining value with Degrees heuristic and Least constraining Value heuristic, Min conflicts strategy etc. 
  * **Propositional logic**: Knowledge bases and inference procedures (Resolution, DPLL, Walksat etc.)
  * **First Order Logic**: Knowledge bases and inference procedures (Model elimination, Otter etc.)
  * **Probabilistic programming**: Static and dynamic Bayesian networks, hidden markov models, exact and approximate inference: rejection sampling, Likelihood weighting, Gibbs sampling, Forward backward, Fixed lag smoothing, particle filtering etc.
  * **Game of GO**: service to generate a training data set from a set of professional go records leveraging [GnuGo](https://www.gnu.org/software/gnugo/) and [Gotraxx](https://gotraxx.codeplex.com), together with a sample data set of 1500 game records. Convoluted neural network with training configuration, together with a sample trained model, and methods to run the model to play or to generate influence maps. 
AI controllers include:
  * Demos with default entities that can be edited online (propositional and first order logic knowledge bases, Bayesian networks, hidden Markov models).
  * Multiple inference strategies, which can be passed as web service parameters to the demos, and support the coursework.
  * Open actions, where the demo entities are replaced by dedicated parameters passed over Http by the web services client. those should ease integrating AI components in your own applications.


## HttpHandlers

HttpHandlers are the most general and flexible components to respond to HttpRequests. 
Dynamic Handlers based on the core engine let you define template based pages, streaming services, or any kind of hosted web server component.

Samples Handlers include:

* **KML WMS Service** - A fully functional tiled WMS service leveraging the SharpMap GIS components suite, and serving layers of Google format KML files from DNN file system.

* **Private Torrent tracker** - A standalone torrent tracker supporting announce and scrap torrent client requests, built with the Monotorrent library.
* **Streaming Proxy**: Efficient buffered multipurpose Http Proxy, Associated to a dedicated firewall rule it can process streamed content to proxify inner urls for instance.

## Control Adapters

Control Adapters let you target kinds of ASP.Net controls and pages, by type or by path and extend or override their behavior, globally to the application, or filtered according to your needs, on specific extension points made of their life cycle and their child controls' life cycle events.

Samples Adapters include:

* **ReCaptcha Adapter** - An adapter that overrides the default DNN Captcha, known for a vulnerability, and replaces it with Google ReCaptcha.

* **Event Viewer page Size adapter** - A simple yet convenient adapter to the event viewer admin DNN module, that saves the results page size in the user personnalization profile, to override the default 10 items result.

* **Asset manager file editor** - In order to simplify editing Skin source files online, a control adapter was introduced to add a file editor to the asset manager, by hijacking the existing module creator's source editor with color syntax. 

* **UrlControl Adapter** - An adapter that enhances the original DNN file selector with an image picker from a 3rd party library.

* **Keep alive adapter** - An adapter that enhances the original DNN Keepalive.aspx page, responsible for swallowing restart requests to keep the user experience fast. It preloads user control types and dnn pages lazy loaded properties to accelerate first visits loading times. 

* Several simple adapters demonstrating customization of the rich text editor, skin objects the main Default.aspx page, or the auto login checkbox.

## Application life cycle agent

Similar to firewall, the application engine taps into application life cycle instead of ASP.Net request life cycle.

Default rules include:

* **Application Start and End sequences** : Tap into the init and end sequence to add your own processing and logging actions.
* **Optimized translator environment** : Removes critical change monitoring from all or specific localization folders to allow for smoother online translation sessions.
* **Optimized developer environment** : Brings Edit&Continue capabilities to a range of development scenarios.
* **Critical changes sequence** : Generally difficult to analyze recycling cycle, logs information useful to document possible issues.
The Application engine also contains custom components such as:

* A UI for easy access to compression parameters
* A virtual custom errors handlers, to deal with exceptions status codes, with flexible capabilities.

## Provider based

Condition and Action types are made from easy to implement providers. 
Look at the included sample providers to create your own types of conditions and actions together with a dedicated UI for custom properties.
Included are the following providers:

* **Condition providers**:
* Sub Conditions
* Dynamic expression
* Client Source (Firewall only)
* DNN Page (Firewall only)
* Portal Alias (Firewall only)
* Membership (Firewall only)
* Request caps (Firewall only)
* **Action providers**:
* Log Event
* Send Email
* Web Action
* Define Variables
* Multiple Actions
* Loop/While Actions
* Object Actions
* String Filter
* File Read/ File Write Actions
* File Manager
* Serialize/Deserialize Actions
* Run SQL
* Run Program
* Load/Save User Identity/Personnalization Profile
* Read/Write Google spreadsheets
* Run Python
* Auto Login (Firewall only)
* Log Off (Firewall only)
* Redirect (Firewall only)
* Customize Environment (Firewall only)
* Display Message (Firewall only)
