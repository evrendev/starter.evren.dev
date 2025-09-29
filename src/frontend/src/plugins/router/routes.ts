import { adminRoutes } from "./routes.admin";
import { authRoutes } from "./routes.auth";
import { publicRoutes } from "./routes.public";

export const routes = [...publicRoutes, ...adminRoutes, ...authRoutes];
