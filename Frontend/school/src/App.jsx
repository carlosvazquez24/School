import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Home from './pages/Home'
import Class from './pages/Class'
import Course from './pages/Course'

import CourseCreate from './components/Course/CourseCreate'
import CourseList from './components/Course/CourseList'
import CourseEdit from './components/Course/CourseEdit'

import ClassCreate from './components/Class/ClassCreate'
import ClassList from './components/Class/ClassList'
import ClassEdit from './components/Class/ClassEdit'

import PrivateRoute from './components/Auth/PrivateRoute'
import Unauthorized from './components/Auth/Unathorized'
import Login from './pages/Auth/Login'
import Register from './pages/Auth/Register'


import 'bootstrap/dist/css/bootstrap.min.css';

import Layout from './components/Layout/Layout'
import './components/Layout/Layout.css'


function App() {
  const [count, setCount] = useState(0)

  return (
    <>

      <BrowserRouter>

        <Routes>


          <Route path='/login' element={<Login />} />
          <Route path='/register' element={<Register />} />
          <Route path='/unauthorized' element={<Unauthorized />} />




          <Route element={<PrivateRoute roles={['Admin', 'Teacher', 'User', 'Student']} />}>

            <Route path='/' element={<Layout />}   >

              <Route path='/' element={<Home />} />

              <Route path='/course' element={<Course />}  >

                <Route index element={<CourseList />} />

                <Route path='create' element={<CourseCreate />} />

                <Route path='edit/:id' element={<CourseEdit />} />

              </Route>


              <Route path='/class' element={<Class />}  >

                <Route index element={<ClassList />} />

                <Route path='create' element={<ClassCreate />} />

                <Route path='edit/:id' element={<ClassEdit />} />

              </Route>


            </Route>

          </Route>





        </Routes>


      </BrowserRouter>

    </>
  )
}

export default App