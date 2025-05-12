import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Container, Table, Button } from 'reactstrap';
import Swal from 'sweetalert2';
import { HasRole } from '../Auth/HasRole';

import { AppSettings } from '../AppSettings/appSettings';

const CourseList = () => {
  const [courses, setCourses] = useState([]);

  const token = localStorage.getItem("token");

  const fetchCourses = async () => {
    try {
      const response = await fetch(`${AppSettings.apiUrl}Course`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${AppSettings.token}`
        }
      });

      if (!response.ok) throw new Error('Error fetching courses');
      const data = await response.json();
      setCourses(data);
    } catch (error) {
      console.error(error);
      Swal.fire({
        title: 'Error',
        text: 'Could not load courses.',
        icon: 'error'
      });
    }
  };


  useEffect(() => {
    fetchCourses();
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
          const response = await fetch(`${AppSettings.apiUrl}Course/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${AppSettings.token}` }
          });

          if (!response.ok) throw new Error('Error deleting the course');

          setCourses(courses.filter(course => course.id !== id));

          Swal.fire({
            title: "Deleted",
            text: "The course has been deleted",
            icon: "success",
            timer: 2000,
            showConfirmButton: false
          });
        } catch (error) {
          Swal.fire({
            title: "Error",
            text: "The course could not be deleted.",
            icon: "error"
          });
          console.error(error);
        }
      }
    });
  };

  return (
    <Container className="mt-4">
      <h2 className="mb-4">Courses List</h2>
      <Table striped>
        <thead>
          <tr>
            <th>Name</th>
            <th>Description</th>
            <th>Credits</th>

            {HasRole("Teacher") && (
              <th>Actions</th>
            )}
          </tr>
        </thead>
        <tbody>
          {courses.map(course => (
            <tr key={course.id}>
              <td>{course.name}</td>
              <td>{course.description}</td>
              <td>{course.credits}</td>
              <td>

                {HasRole("Teacher") && (
                  <>
                    <Link to={`edit/${course.id}`}>
                      <Button color="warning" size="sm" className="me-2">Edit</Button>
                    </Link>
                    <Button color="danger" size="sm" onClick={() => handleDelete(course.id)}>Delete</Button>

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

export default CourseList;
