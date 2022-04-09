import './LoginPage.css'

import { Button, Paper, TextField } from "@material-ui/core";
import React, { ReactElement } from "react"
import { useNavigate } from 'react-router-dom';

export function LoginPage() :  ReactElement {
    const _navigate = useNavigate();
    
    return (
        <div className="container">
            <Paper className="paper" elevation={6}>
                <TextField className='field' id="outlined-basic" label="Логин" variant="outlined" required/>
                <TextField className='field' id="outlined-password-input" label="Пароль" type="password" variant="outlined" required/>
                <Button 
                    className='submitBtn' 
                    variant="outlined" 
                    color="primary"
                    onClick={()=>{_navigate("/menu");}}
                >Вход</Button>
            </Paper>
        </div>
    );
}
