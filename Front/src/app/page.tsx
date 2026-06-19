import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { COOKIE_TOKEN, COOKIE_ROLE } from "@/constants/cookies";
import { APP_ROUTES } from "@/constants/routes";

export default async function HomePage() {
  const cookieStore = await cookies();
  const token = cookieStore.get(COOKIE_TOKEN)?.value;
  const role = cookieStore.get(COOKIE_ROLE)?.value;

  if (!token) redirect(APP_ROUTES.LOGIN);
  if (role === "Admin") redirect(APP_ROUTES.ADMIN);
  redirect(APP_ROUTES.USER);
}
