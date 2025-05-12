import { Outlet,Link, useLocation } from "react-router-dom"
import { HasRole } from "../components/Auth/HasRole";

const Class = () => {

    const location = useLocation();
    return (

        <>
            <h2 className="section-title" > Class </h2>

            <nav className="minimal-nav" >
                <Link className={`nav-item ${location.pathname === "/class" ? "active" : ""}`} to="/class"  > Class list   </Link>


                {HasRole("Teacher") && (<Link className={`nav-item ${location.pathname === "/class/create" ? "active" : ""}`} to="/class/create"  > Create class </Link>)}


            </nav>

            <Outlet/>
        </>


    )
}

export default Class