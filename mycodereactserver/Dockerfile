# Stage 1: Build the React app
FROM node:20.11 AS build
WORKDIR /app

# Copy package.json and package-lock.json (if present)
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy all other source code
COPY . .

# Build the React app
RUN npm run build

# Stage 2: Serve the app with Nginx
FROM nginx:alpine

# Copy the build output to Nginx's html directory
COPY --from=build /app/dist /usr/share/nginx/html

# Copy SSL certificate and key
COPY --from=build /app/certs/mycodereactserver.pem /etc/nginx/ssl/mycodereactserver.pem
COPY --from=build /app/certs/mycodereactserver.key /etc/nginx/ssl/mycodereactserver.key

# Remove the default nginx.conf
RUN rm /etc/nginx/conf.d/default.conf

# Copy your custom nginx.conf
COPY nginx.conf /etc/nginx/conf.d/nginx.conf

# Expose port 80 to the outside world
EXPOSE 3000

# Start Nginx server
CMD ["nginx", "-g", "daemon off;"]

# Set environment variables from .env file
ARG VITE_FRONTEND_URL
ENV VITE_FRONTEND_URL $VITE_FRONTEND_URL
ARG VITE_FRONTEND_PORT
ENV VITE_FRONTEND_PORT $VITE_FRONTEND_PORT
ARG VITE_BACKEND_URL
ENV VITE_BACKEND_URL $VITE_BACKEND_URL
ARG VITE_BACKEND_PORT
ENV VITE_BACKEND_PORT $VITE_BACKEND_PORT
