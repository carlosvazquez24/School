import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { AppSettings } from '../AppSettings/appSettings';
import Swal from 'sweetalert2';

const CourseCreate = () => {
    const [course, setCourse] = useState({ name: '', description: '', credits: 0 });
    const navigate = useNavigate();
  

    const handleChange = (e) => {
        // Obtenemos el nombre del campo y su valor
        const { name, value } = e.target;
      
        // Actualizamos solo el campo que ha cambiado
        setCourse({
          ...course,           // Mantenemos el resto de los valores sin cambios
          [name]: value        // Actualizamos el campo que cambió
        });
      };

    /*
    const handleChange = (e) => {
      setCourse({ ...course, [e.target.name]: e.target.value });
    };
    */
  
    const handleSubmit = async (e) => {
      e.preventDefault();
      try {
        const response = await fetch(`${AppSettings.apiUrl}Course`, {
          method: 'POST',
          headers: { 
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${AppSettings.token}`
          },
          body: JSON.stringify(course),
        });

        if (!response.ok) throw new Error('Error creating course');
        navigate('/course');
      } catch (error) {
        console.error(error);
        Swal.fire({ title: 'Error', text: 'Could not create course.', icon: 'error' });
      }
    };
  
    return (
      <Container className="mt-4">
        <h2>Create Course</h2>
        <Form onSubmit={handleSubmit}>
          <FormGroup>
            <Label for="name">Name</Label>
            <Input type="text" name="name" value={course.name} onChange={handleChange} required />
          </FormGroup>
          <FormGroup>
            <Label for="description">Description</Label>
            <Input type="text" name="description" value={course.description} onChange={handleChange} />
          </FormGroup>
          <FormGroup>
            <Label for="credits">Credits</Label>
            <Input type="number" name="credits" value={course.credits} onChange={handleChange} required />
          </FormGroup>
          <Button color="primary" type="submit">Create</Button>
        </Form>
      </Container>
    );
  };
  

export default CourseCreate;
