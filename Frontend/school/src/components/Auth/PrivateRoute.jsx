// src/components/Auth/PrivateRoute.jsx
import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

const PrivateRoute = ({ roles = [] }) => {
  const location = useLocation();
  const token = localStorage.getItem('token');

  if (!token) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  try {
    const decodedToken = jwtDecode(token);
    const currentTime = Date.now() / 1000;

    if (decodedToken.exp < currentTime) {
      localStorage.removeItem('token');
      return <Navigate to="/login" replace />;
    }

    // Extrae los roles del token
    const userRoles = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    // Asegura que sea un array (por si el usuario tiene solo 1 rol)
    const roleList = Array.isArray(userRoles) ? userRoles : [userRoles];

    // Si se requieren roles específicos, verifica que el usuario tenga al menos uno
    if (roles.length > 0 && !roles.some(role => roleList.includes(role))) {
      return <Navigate to="/unauthorized" replace />;
    }

    return <Outlet />;
  } catch (error) {
    console.error("Error decodificando el token:", error);
    localStorage.removeItem('token');
    return <Navigate to="/login" replace />;
  }
};

export default PrivateRoute;
