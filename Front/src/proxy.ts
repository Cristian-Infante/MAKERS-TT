import { NextRequest, NextResponse } from "next/server";
import { COOKIE_TOKEN, COOKIE_ROLE } from "@/constants/cookies";
import { APP_ROUTES } from "@/constants/routes";

export function proxy(req: NextRequest) {
  const { pathname } = req.nextUrl;
  const token = req.cookies.get(COOKIE_TOKEN)?.value;
  const role  = req.cookies.get(COOKIE_ROLE)?.value;

  const isAuth   = Boolean(token);
  const isAdmin  = role === "Admin";
  const isPublic = pathname === APP_ROUTES.LOGIN ||
                   pathname === APP_ROUTES.REGISTER;

  if (!isAuth) {
    if (isPublic) return NextResponse.next();
    return NextResponse.redirect(new URL(APP_ROUTES.LOGIN, req.url));
  }

  if (isPublic) {
    return NextResponse.redirect(
      new URL(isAdmin ? APP_ROUTES.ADMIN : APP_ROUTES.USER, req.url)
    );
  }

  if (pathname === APP_ROUTES.HOME) {
    return NextResponse.redirect(
      new URL(isAdmin ? APP_ROUTES.ADMIN : APP_ROUTES.USER, req.url)
    );
  }

  if (pathname.startsWith(APP_ROUTES.ADMIN) && !isAdmin) {
    return NextResponse.redirect(new URL(APP_ROUTES.USER, req.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/", "/login", "/register", "/admin/:path*", "/user/:path*"],
};
