import { jwtDecode } from "jwt-decode";

export const getRolesFromToken = () => {
  const token = localStorage.getItem("token");
  if (!token) return [];

  try {
    const decoded = jwtDecode(token);

    // Claim correcto para roles cuando usas ClaimTypes.Role (usando .NET)
    const rawRoles = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    const roles = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
    return roles;
  } catch (err) {
    console.error("Token inválido", err);
    return [];
  }
};

export const HasRole = (role) => {
  const roles = getRolesFromToken();
  return roles.includes(role);
};
