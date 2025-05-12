import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Container, Table, Button } from 'reactstrap';
import Swal from 'sweetalert2';
import { AppSettings } from '../AppSettings/appSettings';

import { HasRole } from '../Auth/HasRole';

const ClassList = () => {
    const [classes, setClasses] = useState([]);

    const token = localStorage.getItem("token");

    const fetchClasses = async () => {
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${AppSettings.apiUrl}Class`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) throw new Error('Error fetching classes');
            const data = await response.json();
            setClasses(data);
        } catch (error) {
            console.error(error);
            Swal.fire({
                title: 'Error',
                text: 'Could not load classes.',
                icon: 'error'
            });
        }
    };


    useEffect(() => {
        fetchClasses();
    }, []);

    const handleDelete = async (id) => {
        Swal.fire({
            title: "Are you sure?",
            text: "This action cannot be undone.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete",
            cancelButtonText: "Cancel"
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    const response = await fetch(`${AppSettings.apiUrl}Class/${id}`,
                        {
                            method: 'DELETE',
                            headers: { 'Authorization': `Bearer ${AppSettings.token}` }

                        });

                    if (!response.ok) throw new Error('Error deleting the class');

                    setClasses(classes.filter(classForm => classForm.id !== id));

                    Swal.fire({
                        title: "Deleted",
                        text: "The class has been deleted",
                        icon: "success",
                        timer: 2000,
                        showConfirmButton: false
                    });
                } catch (error) {
                    Swal.fire({
                        title: "Error",
                        text: "The class could not be deleted.",
                        icon: "error"
                    });
                    console.error(error);
                }
            }
        });
    };

    return (
        <Container className="mt-4">
            <h2 className="mb-4">Class List</h2>


            <Table striped className="mt-3">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Schedule</th>
                        <th>Room</th>
                        <th>Period</th>
                        <th>Course</th>
                        <th>Teacher</th>


                        {HasRole("Teacher") && (
                            <th>Actions</th>

                        )}
                    </tr>
                </thead>
                <tbody>
                    {classes.map(c => (
                        <tr key={c.id}>
                            <td>{c.id}</td>
                            <td>{c.schedule}</td>
                            <td>{c.room}</td>
                            <td>{c.enumPeriod}</td>
                            <td>{c.courseName}</td>
                            <td>{c.teacherName}</td>
                            <td>


                                {HasRole("Teacher") && (
                                    <>
                                        <Link to={`edit/${c.id}`}>
                                            <Button color="warning" size="sm" className="me-2">Edit</Button>
                                        </Link>
                                        <Button color="danger" size="sm" onClick={() => handleDelete(c.id)}>Delete</Button>
                                    </>

                                )}




                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>

        </Container>
    );
};

export default ClassList;
