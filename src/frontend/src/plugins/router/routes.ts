import { adminRoutes } from "./routes.admin";
import { authRoutes } from "./routes.auth";
import { defaultRoutes } from "./routes.default";

export const routes = [...defaultRoutes, ...adminRoutes, ...authRoutes];
