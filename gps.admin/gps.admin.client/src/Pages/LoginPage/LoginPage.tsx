import './LoginPage.css'

import { Button, InputLabel, Paper, TextField } from "@material-ui/core";
import React, { ReactElement, useEffect, useState } from "react"
import { useNavigate } from 'react-router-dom';
import { Api } from '../../Api/Api';
import { Category, Label } from '@material-ui/icons';
import axiosInstance from '../../Axios/AxiosSetup';
import { AxiosError } from 'axios';
import { ErrorConverter } from '../../Axios/AxsiosExtensions';

export function LoginPage(): ReactElement {
    const _navigate = useNavigate();
    const [login, setLogin] = useState("");
    const [isLoginInvalid, setLoginError] = useState(false);
    const [isPasswordInvalid, setPasswordError] = useState(false);
    const [password, setPassword] = useState("");
    const [loginEnabled, setLoginEnabled] = useState(true);
    const [requestStatus, setRequestStatus] = useState("");

    useEffect(() => {
        const refreshToken = localStorage.getItem('token');
        if (refreshToken) {
            _navigate("/menu");
        }
    }, []);

    const onLoginChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setLogin(e.target.value);
        setLoginError(e.target.value.length <= 3);
        setPasswordError(password.length <= 5);
        setLoginEnabled((isLoginInvalid || isPasswordInvalid));
    }

    const onPasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(e.target.value);
        setLoginError(login.length <= 3);
        setPasswordError(e.target.value.length <= 5);
        setLoginEnabled((isLoginInvalid || isPasswordInvalid));
    }

    const onLoginClick = async () => {
        try {
            const data = await Api.loginUser(login, password);
            if (data.user.role == 0) {
                setRequestStatus("У вас нет доступа. Требуется роль 'Администратора'");
            }
            else if (data.token) {
                _navigate("/menu");
            }
        }
        catch (e: AxiosError | any) {
            const error = ErrorConverter(e);
            if (error.status == 401) {
                setRequestStatus(error.message);
            }
            else if (error.status == 403) {
                setRequestStatus("У вас нет доступа!");
            }
            else if (error.status == 400) {
                setRequestStatus("Не корректный запрос. " + error.message);
            }
            else {
                setRequestStatus(error.message);
                console.error(error);
            }
        }
    }

    return (
        <div className="container">
            <Paper className="paper" elevation={6}>
                <TextField
                    className='field'
                    id="outlined-basic"
                    label="Логин"
                    variant="outlined"
                    required
                    value={login}
                    error={isLoginInvalid}
                    onChange={onLoginChange} />
                <TextField
                    className='field'
                    id="outlined-password-input"
                    label="Пароль"
                    type="password"
                    variant="outlined"
                    required
                    value={password}
                    error={isPasswordInvalid}
                    onChange={onPasswordChange} />
                <Button
                    className='submitBtn'
                    variant="outlined"
                    color="primary"
                    disabled={(loginEnabled)}
                    onClick={onLoginClick}
                >Вход</Button>
                <InputLabel className='field' error={true} variant="outlined" >{requestStatus}</InputLabel>
            </Paper>
        </div>
    );
}
