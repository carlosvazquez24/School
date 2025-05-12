import { Outlet, Link, useLocation } from "react-router-dom"
import { HasRole } from "../components/Auth/HasRole";

const Course = () => {

    const location = useLocation();

    return (

        <>
            <h2 className="section-title" > Course </h2>

            <nav className="minimal-nav" >
                <Link className={`nav-item ${location.pathname === "/course" ? "active" : ""}`} to="/course"  > Courses List   </Link>


                {HasRole("Teacher")   && ( <Link className={`nav-item ${location.pathname === "/course/create" ? "active" : ""}`} to="/course/create"  > Create Course </Link>)}


            </nav>

            <Outlet />


        </>


    )
}

export default Course