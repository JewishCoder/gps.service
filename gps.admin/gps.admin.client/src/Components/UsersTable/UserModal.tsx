import {
    Button,
    createStyles,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    FormControl,
    InputLabel,
    makeStyles,
    MenuItem,
    Select,
    TextField,
    Theme,
    Tooltip
} from "@material-ui/core";
import React, { useEffect, useState } from "react"
import { IUser } from "./UsersTable"

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            display: 'flex',
            flexWrap: 'wrap',
            alignItems: 'center',
            justifyContent: 'left',
        },
        textField: {
            marginTop: '10px',
        },
        select: {
            marginTop: '20px'
        }
    }),
);

interface IUserModalProps {
    isOpen: boolean;
    title: string;
    user: IUser | null;
    onSave: (value: IUser, password: string | null) => void;
    onCancel: () => void;
}

const defaultUser = { id: undefined, login: undefined, name: undefined, role: 0 };

export function UserModal({ isOpen, title, user, onSave, onCancel }: IUserModalProps) {
    const classes = useStyles();

    const [newUser, setUser] = useState<IUser>(user ?? defaultUser);
    const [password, setPassword] = useState("");
    const [isLoginInvalid, setLoginError] = useState(false);
    const [isNameInvalid, setNameError] = useState(false);
    const [isPassordInvalid, setPassordError] = useState(false);
    const [cannotSave, setCanSave] = useState(true);

    useEffect(() => {
        if (user) {
            setUser(user);
        }
    }, [user]);

    useEffect(() => {
        if(newUser != defaultUser) {
            setCanSave(getModelErrorStatus());
        }
        else {
            setCanSave(true);
        }
    }, [newUser])

    const getModelErrorStatus = (): boolean => {
        return isLoginInvalid || isNameInvalid || isPassordInvalid;
    }

    const onChangeName = (e: React.ChangeEvent<HTMLInputElement>) => {
        setUser(({ ...newUser, name: e.target.value }));
        setNameError(e.target.value.length == 0);
    }

    const onChangeLogin = (e: React.ChangeEvent<HTMLInputElement>) => {
        setUser(({ ...newUser, login: e.target.value }));
        setLoginError(e.target.value.length <= 3);
    }

    const onChangeRole = (event: React.ChangeEvent<{ value: unknown }>) => {
        setUser(({ ...newUser, role: event.target.value as number }));
    };

    const onChangePassword = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(e.target.value);
        setPassordError(e.target.value.length <= 5);
    }

    const onSaveClick = () => {
        if (newUser && !cannotSave) {
            onSave(newUser, password);
        }
        setUser(defaultUser);
        setPassword("");
        
    }

    const onCancelClick = () => {
        setUser(defaultUser);
        onCancel();
        setPassword("");
    }

    return (
        <Dialog
            open={isOpen}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
        >
            <DialogTitle id="alert-dialog-title" >{title}</DialogTitle>
            <DialogContent  >
                <FormControl fullWidth>
                    <TextField
                        className={classes.textField}
                        name="login"
                        label="Логин"
                        variant="outlined"
                        helperText="Логин не может быть меньше 4 символов"
                        required
                        autoFocus
                        error={isLoginInvalid}
                        value={newUser.login}
                        onChange={onChangeLogin} />
                </FormControl>
                <FormControl fullWidth>
                    <TextField
                        className={classes.textField}
                        name="name"
                        label="Имя"
                        variant="outlined"
                        required
                        error={isNameInvalid}
                        value={newUser.name}
                        onChange={onChangeName} />
                </FormControl>
                <FormControl fullWidth className={classes.textField}>
                    <InputLabel required id="role-label">Роль</InputLabel>
                    <Select
                        labelId="role-label"
                        value={newUser.role}
                        name="role"
                        onChange={onChangeRole}>
                        <MenuItem value={0}>Пользователь</MenuItem>
                        <MenuItem value={1}>Администратор</MenuItem>
                    </Select>
                </FormControl>
                <FormControl fullWidth>
                    <TextField
                        className={classes.textField}
                        name="password"
                        label="Пароль"
                        variant="outlined"
                        type="password"
                        helperText="Пароль не может быть меньше 6 символов"
                        required={user == null}
                        error={isPassordInvalid}
                        value={password}
                        onChange={onChangePassword} />
                </FormControl>
            </DialogContent>
            <DialogActions>
                <Button onClick={onCancelClick} color="primary">Отменить</Button>
                <Button onClick={onSaveClick} disabled={cannotSave} color="primary">Сохранить</Button>
            </DialogActions>
        </Dialog>
    );
}