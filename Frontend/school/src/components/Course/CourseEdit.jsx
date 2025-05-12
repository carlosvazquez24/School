import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Container, Button, Form, FormGroup, Label, Input } from "reactstrap";
import Swal from "sweetalert2";

import { AppSettings } from "../AppSettings/appSettings";

const CourseEdit = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [course, setCourse] = useState({ name: '', description: '', credits: 0 });
  const [originalCourse, setOriginalCourse] = useState(null);

  useEffect(() => {
    const fetchCourse = async () => {
      try {
        const response = await fetch(`${AppSettings.apiUrl}Course/${id}`, {
          headers: { 'Authorization': `Bearer ${AppSettings.token}`  },
        });

        if (!response.ok) throw new Error('Error fetching course');
        const data = await response.json();
        setCourse(data);
        setOriginalCourse(data); // Guardamos el estado original para comparar cambios
      } catch (error) {
        console.error(error);
        Swal.fire({ title: 'Error', text: 'Could not load course.', icon: 'error' });
      }
    };
    fetchCourse();
  }, [id]);

  const handleChange = (e) => {
    setCourse({ ...course, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Si aún no se cargó el curso original, no hacemos nada
    if (!originalCourse) return;

    // Construimos el arreglo de operaciones JSON Patch solo con las propiedades modificadas
    const patchData = [];
    if (course.name !== originalCourse.name) {
      patchData.push({ op: "replace", path: "/name", value: course.name });
    }
    if (course.description !== originalCourse.description) {
      patchData.push({ op: "replace", path: "/description", value: course.description });
    }
    if (course.credits !== originalCourse.credits) {
      patchData.push({ op: "replace", path: "/credits", value: course.credits });
    }

    // Puedes optar por no enviar la petición si no hay cambios
    if (patchData.length === 0) {
      Swal.fire({ title: 'Info', text: 'No changes were made, edit something', icon: 'info' });
      return;
    }

    try {
      const response = await fetch(`${AppSettings.apiUrl}Course/${id}`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json-patch+json', 'Authorization': `Bearer ${AppSettings.token}`  },
        body: JSON.stringify(patchData),
      });
      if (!response.ok) throw new Error('Error updating course');
      navigate('/course');
    } catch (error) {
      console.error(error);
      Swal.fire({ title: 'Error', text: 'Could not update course.', icon: 'error' });
    }
  };

  return (
    <Container className="mt-4">
      <h2>Edit Course</h2>
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
        <Button color="secondary" onClick={() => navigate('/course')}>Back</Button>{' '}
        <Button color="primary" type="submit">Save</Button>
      </Form>
    </Container>
  );
};

export default CourseEdit;
