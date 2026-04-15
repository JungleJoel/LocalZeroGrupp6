import { cookies } from "next/headers";
import { API_BASE_URL } from "@/lib/config";

export default async function checkAuth(): Promise<boolean> {
  const cookieStore = await cookies();

  const sessionCookie = cookieStore.get("LocalZeroCookie");

  if (!sessionCookie) {
    return false;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/Auth/check-auth`, {
      cache: "no-cache",
      method: "GET",
      headers: {
        Cookie: `${sessionCookie.name}=${sessionCookie.value}`,
      },
    });

    if (response.ok) {
      return true;
    } else {
      return false;
    }
  } catch (error) {
    console.log(error);
    return false;
  }
}
