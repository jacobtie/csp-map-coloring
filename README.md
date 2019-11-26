# CSP Map Coloring

This solution to the map coloring problem is implemented here as a web application. The directory `map-ui` contains the frontend, a React application. The directory `csp-api` contains the backend which includes the implementation of the CSP algorithm. This is an ASP.NET Core application.

We acknowledge that, because of the ambitious nature of this project, it may be difficult to run the application. Because of this, we have hosted the application at [https://csp-krevat-reynolds.herokuapp.com/](https://csp-krevat-reynolds.herokuapp.com/) though pure backtracking and forward checking do not always yield a solution due to timeouts.

## Running the backend

Please make sure that you have .NET Core 3 installed. Open the `csp-api` folder in VSCode. VSCode will generate a `.vscode` directory, if it is not already there. Inside the generated `launch.json` folder, delete the following block of code:

```json
// Enable launching a web browser when ASP.NET Core starts. For more information: https://aka.ms/VSCode-CS-LaunchJson-WebBrowser
"serverReadyAction": {
    "action": "openExternally",
    "pattern": "^\\s*Now listening on:\\s+(https?://\\S+)"
},
```

In the debug window, run the application in debug mode. An alternative is to run `dotnet run` in the terminal to run the backend. When the backend is running, you can hit the endpoint using Postman or preferably by running the frontend.

## Running the frontend

Please make sure that you have node.js and npm installed. Open the `map-ui` folder in VSCode. Open the terminal inside this folder. Make sure that the terminal is inside the `map-ui` folder. Run the `npm install` command to install all of the dependencies from the `package.json` into the `node_modules` folder. Once this is complete, run `npm start` to start the development server. This will open a web browser and, as long as the backend is running, it will connect to the backend on port 5000.
