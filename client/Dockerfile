# Stage 1: Build the client application
FROM node:alpine AS build

# Set working directory
WORKDIR /app

# Copy package files for better layer caching
COPY package*.json ./

# Install dependencies
RUN npm ci --only=production=false

# Copy source code
COPY . .

# Clear any build caches
RUN rm -rf node_modules/.vite .vite dist

# Define build arguments for environment variables
ARG VITE_SERVER_URL
ARG VITE_INDEXED_DB_VERSION
ARG VITE_API_KEY
ARG VITE_INDEXED_DB_NAME
ARG VITE_AUTH_DOMAIN
ARG VITE_PROJECT_ID
ARG VITE_STORAGE_BUCKET
ARG VITE_MESSAGING_SENDER_ID
ARG VITE_APP_ID
ARG VITE_MEAUSUREMENT_ID
ARG VITE_ENABLE_DEBUG_LOGGING
ARG VITE_API_TIMEOUT
ARG VITE_MAX_API_RETRIES
ARG VITE_ENABLE_TOKEN_MONITOR
ARG VITE_ENABLE_AUTH_MONITOR
ARG VITE_ENABLE_PERFORMANCE_MONITORING
ARG VITE_ENABLE_REACT_DEVTOOLS
ARG VITE_ENABLE_REDUX_DEVTOOLS

# Set environment variables for build
ENV VITE_SERVER_URL=$VITE_SERVER_URL \
    VITE_INDEXED_DB_VERSION=$VITE_INDEXED_DB_VERSION \
    VITE_API_KEY=$VITE_API_KEY \
    VITE_INDEXED_DB_NAME=$VITE_INDEXED_DB_NAME \
    VITE_AUTH_DOMAIN=$VITE_AUTH_DOMAIN \
    VITE_PROJECT_ID=$VITE_PROJECT_ID \
    VITE_STORAGE_BUCKET=$VITE_STORAGE_BUCKET \
    VITE_MESSAGING_SENDER_ID=$VITE_MESSAGING_SENDER_ID \
    VITE_APP_ID=$VITE_APP_ID \
    VITE_MEAUSUREMENT_ID=$VITE_MEAUSUREMENT_ID \
    VITE_ENABLE_DEBUG_LOGGING=$VITE_ENABLE_DEBUG_LOGGING \
    VITE_API_TIMEOUT=$VITE_API_TIMEOUT \
    VITE_MAX_API_RETRIES=$VITE_MAX_API_RETRIES \
    VITE_ENABLE_TOKEN_MONITOR=$VITE_ENABLE_TOKEN_MONITOR \
    VITE_ENABLE_AUTH_MONITOR=$VITE_ENABLE_AUTH_MONITOR \
    VITE_ENABLE_PERFORMANCE_MONITORING=$VITE_ENABLE_PERFORMANCE_MONITORING \
    VITE_ENABLE_REACT_DEVTOOLS=$VITE_ENABLE_REACT_DEVTOOLS \
    VITE_ENABLE_REDUX_DEVTOOLS=$VITE_ENABLE_REDUX_DEVTOOLS \
    NODE_ENV=production

# Build the application
RUN npm run build

# Verify the build output
RUN ls -la /app/dist && \
    echo "Build successful! Checking for index.html..." && \
    test -f /app/dist/index.html && \
    echo "index.html found!" || \
    (echo "ERROR: index.html not found in dist!" && exit 1)

# Stage 2: Serve the application with nginx
FROM nginx:alpine

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built assets from build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Set proper permissions
RUN chmod -R 755 /usr/share/nginx/html && \
    chown -R nginx:nginx /usr/share/nginx/html

# Verify files are copied correctly
RUN ls -la /usr/share/nginx/html && \
    test -f /usr/share/nginx/html/index.html && \
    echo "Files copied successfully!" || \
    (echo "ERROR: Files not copied correctly!" && exit 1)

# Add healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/ || exit 1

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]