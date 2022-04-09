import { ReactElement, useState } from "react";
import {
    Button, Icon, IconButton, makeStyles, Paper, Table, TableBody,
    TableCell, TableContainer, TableHead, TableRow, Tooltip
} from "@material-ui/core";
import DeleteOutlinedIcon from '@material-ui/icons/DeleteOutlined';
import EditOutlinedIcon from '@material-ui/icons/EditOutlined';
import { UserModal } from "./UserModal";

const useStyles = makeStyles({
    table: {
        height: '100vh',
    },
    header: {
        fontWeight: 'bold',
    },
    button: {
        margin: '0 10px 10px 0',
    },
    idColumn: {
        width: '30px',
    }
});

export interface IUser {
    id: number | undefined;
    login: string| undefined;
    name: string| undefined;
    role: string| undefined;
}

function createData(
    id: number,
    login: string,
    name: string,
    role: string) {
    return { id, login, name, role };
}

const rows = [
    createData(1, 'Admin', 'Администратор', 'Администратор'),
    createData(2, 'test1', 'Тестовый пользак', 'Пользователь'),
    createData(3, 'test2', 'Тестовый пользак', 'Пользователь'),
    createData(4, 'test3', 'Тестовый пользак', 'Пользователь'),
    createData(5, 'test4', 'Тестовый пользак', 'Пользователь'),
];


export function UsersTable(): ReactElement {
    const classes = useStyles();
    const [users, setUsers]=useState<IUser[]>([]);
    const [userModal, setUserModal] = useState<IUser | null>(null);
    const [isUserModalOpen, setIsUserModalOpen] = useState(false);


    const onNewUserSave = (value: IUser) => {
        console.log(value);
        setUsers([...users, value]);
        setIsUserModalOpen(false);
        setUserModal(null);
    }

    const onCancelUserModal=()=>{
        setIsUserModalOpen(false);
        setUserModal(null);
    }

    return (
        <div>
            <Button className={classes.button} variant="outlined" color="primary" onClick={()=>setIsUserModalOpen(true)}>Добавить</Button>
            <TableContainer component={Paper}>
                <Table aria-label="simple table">
                    <TableHead>
                        <TableRow >
                            <TableCell className={classes.idColumn} align="left">Номер</TableCell>
                            <TableCell align="left">Логин</TableCell>
                            <TableCell align="left">Имя</TableCell>
                            <TableCell align="left">Роль</TableCell>
                            <TableCell width={32} ></TableCell>
                            <TableCell width={32}></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {users.map((row) => (
                            <TableRow key={row.id}>
                                <TableCell align="left">{row.id}</TableCell>
                                <TableCell align="left">{row.login}</TableCell>
                                <TableCell align="left">{row.name}</TableCell>
                                <TableCell align="left">{row.role}</TableCell>
                                <TableCell align="left">
                                    <Tooltip title="изменить">
                                        <IconButton aria-label="изменить">
                                            <EditOutlinedIcon />
                                        </IconButton>
                                    </Tooltip>
                                </TableCell>
                                <TableCell align="left">
                                    <Tooltip title="удалить">
                                        <IconButton aria-label="удалить">
                                            <DeleteOutlinedIcon />
                                        </IconButton>
                                    </Tooltip>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <UserModal 
                isOpen={isUserModalOpen} 
                title="Добавление пользователя" 
                user={userModal}
                onSave={onNewUserSave}
                onCancel={onCancelUserModal} />
        </div>
    );
}