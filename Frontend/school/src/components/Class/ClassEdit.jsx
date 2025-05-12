import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Container, Button, Form, FormGroup, Label, Input } from "reactstrap";
import { AppSettings } from "../AppSettings/appSettings";
import Swal from "sweetalert2";

const ClassEdit = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [courses, setCourses] = useState([]);
    

    const periods = [
        { label: 'Semester 1', value: 0 },
        { label: 'Semester 2', value: 1 },
        { label: 'Summer Course', value: 2 },
    ];



    const [classForm, setClassForm] = useState({
        schedule: '',
        room: '',
        enumPeriod: '',
        courseId: ''
    });


    const [originalClass, setOriginalClass] = useState(null);

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
    

    useEffect(() => {
        const fetchCourse = async () => {
            try {
                const response = await fetch(`${AppSettings.apiUrl}Class/${id}`, {
                    headers: {'Authorization': `Bearer ${AppSettings.token}`}  
                });

                if (!response.ok) throw new Error('Error fetching course');
                const data = await response.json();
                setClassForm(data);
                setOriginalClass(data); // Guardamos el estado original para comparar cambios
            } catch (error) {
                console.error(error);
                Swal.fire({ title: 'Error', text: 'Could not load class.', icon: 'error' });
            }
        };
        fetchCourse();
    }, [id]);

    const handleChange = (e) => {
        setClassForm({ ...classForm, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Si aún no se cargó el curso original, no hacemos nada
        if (!originalClass) return;

        // Construimos el arreglo de operaciones JSON Patch solo con las propiedades modificadas
        const patchData = [];

        if (classForm.schedule !== originalClass.schedule) {
            patchData.push({ op: "replace", path: "/schedule", value: classForm.schedule });
        }

        if (classForm.room !== originalClass.room) {
            patchData.push({ op: "replace", path: "/room", value: classForm.room });
        }

        if (classForm.enumPeriod !== originalClass.enumPeriod) {
            patchData.push({ op: "replace", path: "/enumPeriod", value: classForm.enumPeriod });
        }

        if (classForm.courseId !== originalClass.courseId) {
            patchData.push({ op: "replace", path: "/courseId", value: classForm.courseId });
        }


        // Puedes optar por no enviar la petición si no hay cambios
        if (patchData.length === 0) {
            Swal.fire({ title: 'Info', text: 'No changes were made, edit something', icon: 'info' });
            return;
        }

        try {
            const response = await fetch(`${AppSettings.apiUrl}Class/${id}`, {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json-patch+json', 'Authorization': `Bearer ${AppSettings.token}`},
                body: JSON.stringify(patchData),
            });
            if (!response.ok) throw new Error('Error updating class');
            navigate('/class');
        } catch (error) {
            console.error(error);
            Swal.fire({ title: 'Error', text: 'Could not update class.', icon: 'error' });
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

export default ClassEdit;
