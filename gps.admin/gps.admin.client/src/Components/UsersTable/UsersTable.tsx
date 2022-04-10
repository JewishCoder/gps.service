import { ReactElement, useState } from "react";
import {
    Button, IconButton, makeStyles, Paper, Table, TableBody,
    TableCell, TableContainer, TableHead, TableRow, Tooltip
} from "@material-ui/core";
import DeleteOutlinedIcon from '@material-ui/icons/DeleteOutlined';
import EditOutlinedIcon from '@material-ui/icons/EditOutlined';
import { UserModal } from "./UserModal";
import { DeleteConfirmModal } from "../DeleteConfirmModal";

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
    login: string | undefined;
    name: string | undefined;
    role: number;
};

export function UsersTable(): ReactElement {
    const classes = useStyles();
    const [users, setUsers] = useState<IUser[]>([]);
    const [userModal, setUserModal] = useState<IUser | null>(null);
    const [deleteUserModal, setDeleteUserModal] = useState<IUser | null>(null);
    const [isUserModalOpen, setIsUserModalOpen] = useState(false);
    const [isEditUserModalOpen, setIsEditUserModalOpen] = useState(false);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

    const onNewUserSave = (value: IUser, password: string | null) => {
        value.id = users.length + 1;
        setUsers([...users, value]);
        setIsUserModalOpen(false);
        setUserModal(null);
    }

    const onCancelUserModal = () => {
        setIsUserModalOpen(false);
        setUserModal(null);
    }

    const onEditUserSave = (value: IUser, password: string | null) => {
        setUsers(users.map((user) => {
            if (user.id == value.id) {
                return value;
            }
            return user;
        }));
        setIsEditUserModalOpen(false);
        setUserModal(null);
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

    const onUserDeleteYesClick = () => {
        setIsDeleteModalOpen(false);
        setUsers(users.filter(x => x.id != deleteUserModal?.id));
        setDeleteUserModal(null);
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
                                <TableCell align="left">{row.role==0 ? ("Пользователь") : ("Администратор")}</TableCell>
                                <TableCell align="left">
                                    <Tooltip title="изменить">
                                        <IconButton aria-label="изменить" onClick={() => onEditUserClick(row)}>
                                            <EditOutlinedIcon />
                                        </IconButton>
                                    </Tooltip>
                                </TableCell>
                                <TableCell align="left">
                                    <Tooltip title="удалить">
                                        <IconButton aria-label="удалить" onClick={() => onDeleteUserClick(row)}>
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