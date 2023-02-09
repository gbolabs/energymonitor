# Web

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 14.0.2.

## Setup

1. Install [Node.js](https://nodejs.org/en/download/) 
2. Install [Angular CLI](https://angular.io/cli) `npm install -g @angular/cli`
3. Install Static Web Apps CLI: `npm install -g @azure/static-web-apps-cli`

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.

## Deploy to Azure

### Connect to Azure App

```bash
swa login --subscription-id {} \
--resource-group {}} \
--app-name {}}
```

1. Run `npm run build` to build the project. The build artifacts will be stored in the `dist/` directory.
2. Run `swa start http://localhost:4200` to start the Static Web Apps CLI. Navigate to `http://localhost:4280/` to view the application.
3. Run `swa publish` to publish the application to Azure.
4. Run `swa stop` to stop the Static Web Apps CLI.
5. Run `swa delete` to delete the Static Web Apps CLI.
6. Run `swa deploy --deployment-token {token} /dist/web --env {production|preview}` to deploy the application to Azure.
