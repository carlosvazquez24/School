import React, { useState } from "react";
import "./Layout.css";
import { NavLink, Outlet } from "react-router-dom";

import { useNavigate } from "react-router-dom";

const Layout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const navigate = useNavigate();


  const handleLogout = () => {
    localStorage.removeItem('token'); 
    navigate('/login'); 
  };

  return (
    <div className="layout">
      {/* Sidebar */}
      <aside className={`sidebar ${sidebarOpen ? "open" : ""}`}>
        <button className="close-btn" onClick={() => setSidebarOpen(false)}>✖</button>
        <div className="sidebar-header">
          <h2 className="system-name"> School System </h2>
        </div>
        <ul>
          <li>
            <NavLink to="/" end className={({ isActive }) => `nav-link ${isActive ? "active" : ""}`}>
              Home
            </NavLink>
          </li>
          <li>
            <NavLink to="/course" className={({ isActive }) => `nav-link ${isActive ? "active" : ""}`}>
              Course
            </NavLink>
          </li>
          <li>
            <NavLink to="/class" className={({ isActive }) => `nav-link ${isActive ? "active" : ""}`}>
              Class
            </NavLink>
          </li>
          <div className="vertical-line"></div>
          <li><a className="nav-link">Notificaciones</a></li>
          <li><a onClick={handleLogout} className="nav-link">Cerrar sesión</a></li>
        </ul>
      </aside>

      <div className="main-content">
        {/* Header */}
        <header className="header">
          <button className="menu-btn" onClick={() => setSidebarOpen(!sidebarOpen)}>
            ☰
          </button>
        </header>

        {/* Contenido Principal */}
        <main className="content">
          <Outlet />
        </main>

        {/* Footer */}
        <footer className="footer">
        </footer>
      </div>
    </div>
  );
};

export default Layout;
