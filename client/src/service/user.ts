import unfetch from "unfetch";

export const authenticate = async (
  username: string,
  password: string
): Promise<{ status: number; data: string; validTo: string }> => {
  const response = await unfetch(
    "http://34.124.201.193/wp-json/jwt-auth/v1/token",
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ username, password }),
    }
  );

  const data = await response.json();

  if (response.status !== 200) {
    throw new Error(data.error);
  }

  const token = data.token;

  const loginResponse = await unfetch("http://localhost:8082/api/login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ token }),
  });

  const validTo = await loginResponse.json();

  return {
    status: response.status,
    data,
    validTo,
  };
};
