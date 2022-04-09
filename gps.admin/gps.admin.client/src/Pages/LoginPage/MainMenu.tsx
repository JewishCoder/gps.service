import { AppBar, Button, makeStyles, Paper, Tab, Tabs, } from '@material-ui/core';
import { LatLngExpression } from 'leaflet';
import React from 'react';
import { MarkerMap } from '../../Components/Marker/MarkerMap';
import SwitchButton from '../../Components/SwitchButton';
import { TabPanel } from '../../Components/TabPanel';
import { UsersTable } from '../../Components/UsersTable/UsersTable';

const useStyles = makeStyles((thema) => ({
    container: {
        height: '100vh',
        overflow: 'hidden',
    },
    paper: {
        background: 'red',
        margin: '4px'
    },
    switchButton: {
        marginBottom: '10px'
    }
}));


export function MainMenu(): React.ReactElement {

    const position: LatLngExpression = [59.131972, 37.860343]
    const zoom: number = 15.5;

    const classes = useStyles();
    const [value, setValue] = React.useState(0);
    const [editMode, setEditMode] = React.useState(false);
    

    const handleChange = (event: React.ChangeEvent<{}>, newValue: number) => {
        setValue(newValue);
    };

    return (
        <div>
            <AppBar position="static">
                <Tabs value={value} onChange={handleChange}>
                    <Tab label="Точки" />
                    <Tab label="Пользователи" />
                </Tabs>
            </AppBar>
            <TabPanel value={value} index={0}>
                <SwitchButton classesName={classes.switchButton} label={"Режим редактирование"} onChange={setEditMode} />
                <MarkerMap isEditMode={editMode} centerPosition={position} zoom={zoom} />
            </TabPanel>
            <TabPanel value={value} index={1}>
                <UsersTable />
            </TabPanel>
        </div>
    );
}