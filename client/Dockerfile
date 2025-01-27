# Building the client side of the application.
FROM node:22-alpine as build
WORKDIR /client
COPY package*.json .
RUN npm install
COPY . .

ARG VITE_SERVER_URL
ENV VITE_SERVER_URL=$VITE_SERVER_URL
RUN npm run build

# Serving the client side of the application using nginx.
FROM nginx:stable-alpine as production
COPY --from=build /client/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]