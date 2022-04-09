import React from 'react';
import logo from './logo.svg';
import './App.css';
import { Route, Routes } from "react-router-dom";
import { LoginPage } from './Pages/LoginPage/LoginPage';
import { MainMenu } from './Pages/LoginPage/MainMenu';

function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<LoginPage/>} />
        <Route path="/menu" element={<MainMenu/>} />
      </Routes>
    </div>
  );
}

export default App;
