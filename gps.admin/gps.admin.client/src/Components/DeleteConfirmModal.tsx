import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import { makeStyles } from '@material-ui/core';

const useStyles = makeStyles(() => ({
    content: {
        color: "black"
    }
}));

interface IDeleteConfirmModalProps{
    isOpen: boolean;
    data: string;
    onYesClick: () => void;
    onNoClick: () => void;
}

export function DeleteConfirmModal({isOpen, data, onYesClick, onNoClick}: IDeleteConfirmModalProps) {
    const classes = useStyles();
  
    return (
    <Dialog
        open={isOpen}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
     >
        <DialogTitle id="alert-dialog-title">Подтверждение удаления</DialogTitle>
        <DialogContent>
            <DialogContentText className={classes.content} id="alert-dialog-description">
               {data}
            </DialogContentText>
        </DialogContent>
        <DialogActions>
            <Button onClick={onNoClick} color="primary">
                Нет
            </Button>
            <Button onClick={onYesClick} color="primary" autoFocus>
                Да
            </Button>
        </DialogActions>
    </Dialog>
  );
}