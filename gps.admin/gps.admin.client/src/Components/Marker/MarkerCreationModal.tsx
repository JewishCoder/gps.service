import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, TextField } from "@material-ui/core";
import { useEffect, useState } from "react";

interface IDialogProps {
    isOpen: boolean,
    title: string,
    markerName: string,
    onSaveClick: (name: string) => void,
    onCancelClick: () => void,
}

export function MarkerCreationModal({ isOpen, title, markerName, onSaveClick, onCancelClick }: IDialogProps) {
    const [name, setName] = useState<string>(markerName);
    const [nameError, setNameError] = useState(false);

    useEffect(() => {
        setName(markerName);
      }, [markerName]);

    const onSave = () => {
        if (name.length == 0) {
            setNameError(true);
            return;
        }

        setNameError(false);
        onSaveClick(name);
        setName("");
    }

    const onCancel = () => {
        setNameError(false);
        onCancelClick();
    }

    return (
        <Dialog
            open={isOpen}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
        >
            <DialogTitle id="alert-dialog-title">{title}</DialogTitle>
            <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    <TextField
                        id="outlined-basic"
                        label="Наименование"
                        variant="outlined"
                        required
                        autoFocus
                        error={nameError}
                        value={name}
                        onChange={(e) => setName(e.target.value)} />
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={onCancel} color="primary">Отменить</Button>
                <Button onClick={onSave} color="primary">Сохранить</Button>
            </DialogActions>
        </Dialog>
    );
}