FROM node:slim AS build

WORKDIR /app

COPY . .

RUN npm install
RUN npm run build

FROM nginx:stable-alpine-slim
COPY --from=build /app/dist /usr/share/nginx/html