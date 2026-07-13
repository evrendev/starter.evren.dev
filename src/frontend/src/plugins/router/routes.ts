import { adminRoutes } from "./routes.admin";
import { authRoutes } from "./routes.auth";
import { publicRoutes } from "./routes.public";
import { learningRoutes } from "./routes.learning";

export const routes = [
  ...publicRoutes,
  ...adminRoutes,
  ...authRoutes,
  ...learningRoutes,
];
