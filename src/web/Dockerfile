# Stage 1: Build the Angular app
FROM node:22 AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

# Run the build for the fly.io production environment
RUN npm run fly

# Stage 2: Serve the app using Nginx
FROM nginx:latest

RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/dist/fly /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]