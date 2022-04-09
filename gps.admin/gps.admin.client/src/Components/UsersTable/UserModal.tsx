import { Button, createStyles, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, FormControl, InputLabel, makeStyles, MenuItem, Select, TextField, Theme } from "@material-ui/core";
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
    onSave: (value: IUser) => void;
    onCancel: () => void;
}

export function UserModal({ isOpen, title, user, onSave, onCancel }: IUserModalProps) {
    const classes = useStyles();
    
    const [newUser, setUser] = useState<IUser | null>(user);
    const [nameError, setNameError] = useState(false);
    const [age, setAge] = React.useState(0);

    const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
      setAge(event.target.value as number);
    };
   
    useEffect(() => {
        setUser(user);
    }, [user]);


    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
       
        setUser((prev)=>prev?({...prev, name:value}):({id:undefined,login:"", name:value,role:""}));
       
        console.log(newUser);
        
    }

    const handleInputChange1 = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
       
        
        setUser((prev)=>prev?({...prev, login:value}):({id:undefined,name:"", login:value,role:""}));
        console.log(newUser);
    }

    const handleInputChange2 = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
       
        
        
       
    }

    const onSaveClick = () => {
        if (newUser) {
            onSave(newUser);
        }
    }

    const onCancelClick = () => {
        onCancel();
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
                        required
                        autoFocus
                        
                        error={nameError}
                        value={newUser?.login}
                        onChange={handleInputChange1} />
                </FormControl>
                <FormControl fullWidth>
                    <TextField
                        className={classes.textField}
                        name="name"
                        label="Имя"
                        variant="outlined"
                        required
                        error={nameError}
                        value={newUser?.name}
                        onChange={handleInputChange} />
                </FormControl>
                <FormControl fullWidth className={classes.textField}>
                    <InputLabel required id="role-label">Роль</InputLabel>
                    <Select
                        labelId="role-label"
                        value={age}
                        name="role"
                        onChange={handleChange}
                    >
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
                        required
                        error={nameError}
                        value={newUser?.name}
                        onChange={handleInputChange2} />
                </FormControl>
            </DialogContent>
            <DialogActions>
                <Button onClick={onCancelClick} color="primary">Отменить</Button>
                <Button onClick={onSaveClick} color="primary">Сохранить</Button>
            </DialogActions>
        </Dialog>
    );
}