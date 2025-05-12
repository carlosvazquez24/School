import React from 'react';
import { Link } from 'react-router-dom';

const Unauthorized = () => {
  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="card shadow p-4" style={{ maxWidth: '400px' }}>
        <h4 className="text-danger">Acceso denegado</h4>
        <p className="mb-3">No tienes permisos para acceder a esta sección.</p>
        <Link to="/login" className="btn btn-outline-primary">
          Volver al inicio
        </Link>
      </div>
    </div>
  );
};

export default Unauthorized;
