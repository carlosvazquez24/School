import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Container, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import Swal from 'sweetalert2';

import { AppSettings } from '../AppSettings/appSettings';

const ClassCreate = () => {

    const periods = [
        { label: 'Semester 1', value: 0 },
        { label: 'Semester 2', value: 1 },
        { label: 'Summer Course', value: 2 },
    ];


    const [courses, setCourses] = useState([]);


    const [classForm, setClassForm] = useState({
        schedule: '',
        room: '',
        enumPeriod: '',
        courseId: ''
    });


    const navigate = useNavigate();


    const handleChange = (e) => {
        // Obtenemos el nombre del campo y su valor
        const { name, value } = e.target;

        // Actualizamos solo el campo que ha cambiado
        setClassForm({
            ...classForm,           // Mantenemos el resto de los valores sin cambios
            [name]: value        // Actualizamos el campo que cambió
        });
    };


    const fetchCourses = async () => {
        try {

            const response = await fetch(`${AppSettings.apiUrl}Course`, {
                method: 'GET',
                headers: {
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
                text: error.message,
                icon: 'error'
            });
        }
    };


    useEffect(() => {
        fetchCourses();
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            console.log(classForm);
    
            const response = await fetch(`${AppSettings.apiUrl}Class`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${AppSettings.token}`
                },
                body: JSON.stringify(classForm),
            });
    
            if (!response.ok) {
                const errorData = await response.json();
    
                // Verifica si vienen errores de validación
                if (errorData.errors) {
                    const messages = Object.entries(errorData.errors)
                        .flatMap(([field, msgs]) => msgs.map(msg => `${field}: ${msg}`))
                        .join('\n');
    
                    Swal.fire({
                        icon: 'error',
                        title: 'Errores de validación',
                        text: messages,
                        customClass: { popup: 'text-start' }, // Para alinear el texto a la izquierda si lo deseas
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: errorData.error || 'Ocurrió un error desconocido.',
                    });
                }
                return;
            }
    
            navigate('/class');
    
        } catch (error) {
            console.error(error);
            Swal.fire({
                title: 'Error',
                text: "Error al enviar los datos.",
                icon: 'error'
            });
        }
    };
    

    return (
        <Container className="mt-4">
            <h2>Create Class</h2>
            <Form onSubmit={handleSubmit}>
                <FormGroup>
                    <Label for="schedule">Schedule</Label>
                    <Input name="schedule" value={classForm.schedule} onChange={handleChange} required />
                </FormGroup>

                <FormGroup>
                    <Label for="room">Room</Label>
                    <Input name="room" value={classForm.room} onChange={handleChange} required />
                </FormGroup>

                <FormGroup>
                    <Label for="enumPeriod">Period</Label>
                    <Input
                        type="select"
                        name="enumPeriod"
                        value={classForm.enumPeriod}
                        onChange={handleChange}
                    >
                        <option value="">-- Select Period --</option>

                        {periods.map(p => (
                            <option key={p.value} value={p.value}>
                                {p.label}
                            </option>
                        ))}
                    </Input>
                </FormGroup>


                <FormGroup>
                    <Label for="courseId">Course</Label>
                    <Input type="select" name="courseId" value={classForm.courseId} onChange={handleChange} required>
                        <option value="">-- Select Course --</option>
                        {courses.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                    </Input>
                </FormGroup>

                <Button color="primary" type="submit">Save</Button>
            </Form>
        </Container>
    );
};


export default ClassCreate;
