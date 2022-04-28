import { AppBar, Button, makeStyles, Paper, Tab, Tabs, } from '@material-ui/core';
import { HubConnection } from '@microsoft/signalr';
import { LatLngExpression } from 'leaflet';
import React, { useEffect } from 'react';
import { Api } from '../../Api/Api';
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
    const [notifyConnection, setNotifyConnection] = React.useState<HubConnection | null>(null);

    useEffect(() => {
       const connection = Api.getNotifyService();
       if(connection){
           connection.start().then(()=>{
                connection.on("Connected",(data)=>{
                    console.log(data);
                })
           });
           setNotifyConnection(connection);
       }
       else{
           console.log("notify service not builded");
       }
    }, []);

    const handleChange = (event: React.ChangeEvent<{}>, newValue: number) => {
        setValue(newValue);
        if(newValue==1){
            setEditMode(false);
        }
    };

    return (
        <div>
            <AppBar position="static">
                <Tabs value={value} onChange={handleChange}>
                    <Tab label="Точки назначения" />
                    <Tab label="Пользователи" />
                </Tabs>
            </AppBar>
            <TabPanel value={value} index={0}>
                <SwitchButton classesName={classes.switchButton} label={"Режим редактирование"} onChange={setEditMode} />
                <MarkerMap isEditMode={editMode} centerPosition={position} zoom={zoom} notifyService={notifyConnection} />
            </TabPanel>
            <TabPanel value={value} index={1}>
                <UsersTable notifyService={notifyConnection} />
            </TabPanel>
        </div>
    );
}