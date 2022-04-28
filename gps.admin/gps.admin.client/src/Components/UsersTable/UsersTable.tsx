import { ReactElement, useEffect, useState } from "react";
import {
    Button, IconButton, makeStyles, Paper, Table, TableBody,
    TableCell, TableContainer, TableHead, TableRow, Tooltip
} from "@material-ui/core";
import DeleteOutlinedIcon from '@material-ui/icons/DeleteOutlined';
import EditOutlinedIcon from '@material-ui/icons/EditOutlined';
import { UserModal } from "./UserModal";
import { DeleteConfirmModal } from "../DeleteConfirmModal";
import { Api } from "../../Api/Api";
import { useNavigate } from "react-router-dom";
import { AxiosError } from "axios";
import { ErrorConverter } from "../../Axios/AxsiosExtensions";
import { HubConnection } from "@microsoft/signalr";

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

interface IUserTableProps {
    notifyService: HubConnection | null,
}
export interface IUser {
    id: number | undefined;
    login: string | undefined;
    name: string | undefined;
    role: number;
};

export function UsersTable({ notifyService }: IUserTableProps): ReactElement {
    const _navigate = useNavigate();
    const classes = useStyles();
    const [users, setUsers] = useState<IUser[]>([]);
    const [userModal, setUserModal] = useState<IUser | null>(null);
    const [deleteUserModal, setDeleteUserModal] = useState<IUser | null>(null);
    const [isUserModalOpen, setIsUserModalOpen] = useState(false);
    const [isEditUserModalOpen, setIsEditUserModalOpen] = useState(false);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

    const fetchData = async () => {
        try {
            const data = await Api.getUsers();
            setUsers(data as IUser[]);
        }
        catch (e: AxiosError | any) {
            const error = ErrorConverter(e);
            if (error.status == 401) {
                _navigate("/");
            }
            else if (error.status == 403) {
                alert("У вас нет доступа!");
            }
            else if (error.status == 400) {
                alert("Не корректный запрос. " + error.message);
            }
            else {
                alert(error.message);
                console.error(error);
            }
        }
    }

    useEffect(() => {
        notifyService?.on("NewUserEvent", (user) => {
            const userId = Api.getUserId();
            if (user && user.newUserId != userId) {
                fetchData();
            }
        });
        notifyService?.on("UpdatedUserEvent", (user) => {
            const userId = Api.getUserId();
            if (user && user.updatedUserId != userId) {
                fetchData();
            }
        });
        notifyService?.on("DeletedUserEvent", (user) => {
            const userId = Api.getUserId();
            if (user) {
                if (user.deletedUserId != userId) {
                    fetchData();
                }
                else {
                    alert("Вас исключил из системы пользователь " + user.login);
                    localStorage.removeItem('token');
                    _navigate("/");
                }
            }
        });
        fetchData();
    }, [notifyService]);

    const onNewUserSave = async (value: IUser, password: string | null) => {
        if (password) {
            try {
                const data = await Api.createUser(value, password);
                setUsers([...users, data as IUser]);
                setIsUserModalOpen(false);
                setUserModal(null);
            }
            catch (e: AxiosError | any) {
                const error = ErrorConverter(e);
                if (error.status == 401) {
                    _navigate("/");
                }
                else if (error.status == 403) {
                    alert("У вас нет доступа!");
                }
                else if (error.status == 400) {
                    alert("Не корректный запрос. " + error.message);
                }
                else {
                    alert(error.message);
                    console.error(error);
                }
            }
        }
    }

    const onCancelUserModal = () => {
        setIsUserModalOpen(false);
        setUserModal(null);
    }

    const onEditUserSave = async (value: IUser, password: string | null) => {
        if (value.id && value.id <= 0) return;
        try {
            const data = await Api.updateUser(value, password);
            const user = data as IUser;
            setUsers(users.map((x) => {
                if (x.id == user.id) {
                    return user;
                }
                return x;
            }));
            setIsEditUserModalOpen(false);
            setUserModal(null);
        }
        catch (e: AxiosError | any) {
            const error = ErrorConverter(e);
            if (error.status == 401) {
                _navigate("/");
            }
            else if (error.status == 403) {
                alert("У вас нет доступа!");
            }
            else if (error.status == 400) {
                alert("Не корректный запрос. " + error.message);
            }
            else {
                alert(error.message);
                console.error(error);
            }
        }
    }

    const onCancelEditUserModal = () => {
        setIsEditUserModalOpen(false);
        setUserModal(null);
    }

    const onEditUserClick = (user: IUser) => {
        setUserModal(user);
        setIsEditUserModalOpen(true);
    }

    const onDeleteUserClick = (user: IUser) => {
        setDeleteUserModal(user);
        setIsDeleteModalOpen(true);
    }

    const onUserDeleteYesClick = async () => {
        if (deleteUserModal && deleteUserModal.id && deleteUserModal.id > 0) {
            try {
                const data = await Api.deleteUser(deleteUserModal.id);
                if (data == true) {
                    setIsDeleteModalOpen(false);
                    setUsers(users.filter(x => x.id != deleteUserModal.id));
                    setDeleteUserModal(null);
                }
                else {
                    alert("Не удалось удалить пользователя!");
                }
            }
            catch (e: AxiosError | any) {
                const error = ErrorConverter(e);
                if (error.status == 401) {
                    _navigate("/");
                }
                else if (error.status == 403) {
                    alert("У вас нет доступа!");
                }
                else if (error.status == 400) {
                    alert("Не корректный запрос. " + error.message);
                }
                else {
                    alert(error.message);
                    console.error(error);
                }
            }
        }
    }

    const onUserDeleteNoClick = () => {
        setIsDeleteModalOpen(false);
        setDeleteUserModal(null);
    }

    return (
        <div>
            <Button
                className={classes.button}
                variant="outlined"
                color="primary"
                onClick={() => setIsUserModalOpen(true)}>Добавить</Button>
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
                                <TableCell align="left">{row.role == 0 ? ("Пользователь") : ("Администратор")}</TableCell>
                                <TableCell align="left">
                                    <Tooltip title="изменить">
                                        <IconButton aria-label="изменить"  disabled={row.id == 1} onClick={() => onEditUserClick(row)}>
                                            <EditOutlinedIcon />
                                        </IconButton>
                                    </Tooltip>
                                </TableCell>
                                <TableCell align="left">
                                    <Tooltip title="удалить">
                                        <IconButton aria-label="удалить" disabled={row.id == 1 || row.id == Api.getUserId()} onClick={() => onDeleteUserClick(row)}>
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
            <UserModal
                isOpen={isEditUserModalOpen}
                title="Изменение пользователя"
                user={userModal}
                onSave={onEditUserSave}
                onCancel={onCancelEditUserModal} />
            <DeleteConfirmModal
                isOpen={isDeleteModalOpen}
                data="Вы уверены, что хотите удалить пользователя?"
                onYesClick={onUserDeleteYesClick}
                onNoClick={onUserDeleteNoClick} />
        </div>
    );
}

