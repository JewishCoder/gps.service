import { Button, ButtonGroup, makeStyles } from "@material-ui/core";
import { HubConnection } from "@microsoft/signalr";
import { AxiosError } from "axios";
import { LatLng, LatLngExpression, LatLngLiteral, LeafletMouseEvent } from "leaflet";
import { ReactElement, useEffect, useState } from "react";
import { MapContainer, Marker, Popup, TileLayer, Tooltip } from "react-leaflet";
import { useNavigate } from "react-router-dom";
import { Api, IPoint } from "../../Api/Api";
import { ErrorConverter } from "../../Axios/AxsiosExtensions";
import { DeleteConfirmModal } from "../DeleteConfirmModal";
import { MarkerCreationModal } from "./MarkerCreationModal";
import { MarkerItem } from "./MarkerItem";


const useStyles = makeStyles(() => ({
    container: {
        width: '100%',
        height: '80vh',
    },
    dialog: {
        position: 'absolute',
        width: '300px',
        height: '400px',
    }
}));


interface IMarkerSate {
    id: number,
    name: string,
    position: LatLngExpression,
}

interface IMarkerMapProps {
    isEditMode: boolean,
    centerPosition: LatLngExpression,
    zoom: number,
    notifyService: HubConnection | null,
}

export function MarkerMap({ isEditMode, centerPosition, zoom, notifyService }: IMarkerMapProps): ReactElement {
    const _navigate = useNavigate();
    const classes = useStyles();

    const [markers, setMarkers] = useState<IMarkerSate[]>([]);
    const [newMarkerDialogState, setNewMarkerDialogState] = useState(false);
    const [updateMarkerDialogState, setUpdateMarkerDialogState] = useState(false);
    const [deleteMarkerDialogState, setDeleteMarkerDialogState] = useState(false);
    const [newPosition, setNewPosition] = useState<LatLngExpression | null>(null);
    const [initDialogMarker, setinitDialogMarker] = useState<IMarkerSate | null>(null);
    const [deleteMarker,  setDeleteMarker] = useState<IMarkerSate | null>(null);

    const fetchPoints = async () => {
        try {
            const data = await Api.getPoints();
            const markers = data.map<IMarkerSate>(x => {
                return {
                    id: x.id,
                    name: x.name,
                    position: [x.latitude, x.longitude] as LatLngExpression
                };
            })
            setMarkers(markers);
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
        notifyService?.on("NewPointEvent", (point) => {
            const userId = Api.getUserId();
            if (point && point.userId != userId) {
                fetchPoints();
            }
        });
        notifyService?.on("UpdatedPointEvent", (point) => {
            const userId = Api.getUserId();
            if (point && point.userId != userId) {
                fetchPoints();
            }
        });
        notifyService?.on("DeletedPointEvent", (point) => {
            const userId = Api.getUserId();
            if (point && point.userId != userId) {
                fetchPoints();
            }
        });
        fetchPoints();
    }, [notifyService]);

    const addMarker = (marker: IMarkerSate) => {
        setMarkers(markers => [...markers, marker]);
    }

    const removeMarker = async (marker: IMarkerSate) => {
       setDeleteMarker(marker);
       setDeleteMarkerDialogState(true);
    }

    const updateMarker = (marker: IMarkerSate) => {
        setinitDialogMarker(marker);
        setUpdateMarkerDialogState(true);
    }

    const mapClick = (e: LeafletMouseEvent) => {
        if (isEditMode) {
            setNewPosition(e.latlng);
            setNewMarkerDialogState(true);
        }
    };

    const onNewMarkerDialogCancelClick = () => {
        setNewPosition(null);
        setNewMarkerDialogState(false);
    }

    const onNewMarkerDialogSaveClick = async (name: string) => {
        if (newPosition) {
            try {
                const data = await Api.createPoint(name, newPosition as LatLng);
                addMarker({
                    id: data.id,
                    name: data.name,
                    position: [data.latitude, data.longitude] as LatLngExpression
                });
                setNewPosition(null);
                setNewMarkerDialogState(false);
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
        else {
            console.warn('marker position not found');
        }
    }

    const onUpdateMarkerDialogCancelClick = () => {
        setinitDialogMarker(null);
        setUpdateMarkerDialogState(false);
    }

    const onUpdateMarkerDialogSaveClick = async (name: string) => {
        if (initDialogMarker == null) return;
        try{
            const data = await Api.updatePoint(initDialogMarker.id, name, initDialogMarker.position as LatLng);
            const updatedMarkes = markers.map((item) => {
                if (item.id == data.id) {
                    return {
                        id: data.id,
                        name: data.name,
                        position: [data.latitude, data.longitude] as LatLngExpression
                    };
                }

                return item;
            });

            setMarkers(updatedMarkes);
            setinitDialogMarker(null);
            setUpdateMarkerDialogState(false);
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

    const onDeleteYesClick = async ()  => {
        
        try {
            if(deleteMarker==null) return;
            const data = await Api.deletePoint(deleteMarker.id);
            if (data) {
                setMarkers(markers.filter(x => x.id != deleteMarker.id));
            }
            else {
                alert("Не удалось удалить точку!");
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
        finally {
            setDeleteMarkerDialogState(false);
            setDeleteMarker(null);
        }
        
    }

    const onDeleteNoClick = () : void => {
        setDeleteMarkerDialogState(false);
        setDeleteMarker(null);
    }

    return (
        <>
            <MarkerCreationModal isOpen={newMarkerDialogState} title={"Создание точки назначения"} markerName={""} onSaveClick={onNewMarkerDialogSaveClick} onCancelClick={onNewMarkerDialogCancelClick} />
            <MarkerCreationModal isOpen={updateMarkerDialogState} title={"Изменение точки назначения"} markerName={initDialogMarker?.name ?? ""} onSaveClick={onUpdateMarkerDialogSaveClick} onCancelClick={onUpdateMarkerDialogCancelClick} />
            <DeleteConfirmModal isOpen={deleteMarkerDialogState} data="Вы уверены, что хотите удалить точку назначения?" onYesClick = {onDeleteYesClick} onNoClick = {onDeleteNoClick}/>
            <MapContainer
                className={classes.container}
                center={centerPosition} zoom={zoom}>
                <MarkerItem onMapClick={mapClick} />
                <TileLayer
                    noWrap={true}
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                {
                    markers.map((marker, index) => (
                        <Marker position={marker.position} key={marker.id}>
                            <Tooltip permanent>
                                <span>{marker.name}</span>
                            </Tooltip>
                            <Popup>
                                <Button disabled={!isEditMode} size="small" onClick={() => updateMarker(marker)}>Изменить</Button><br />
                                <Button disabled={!isEditMode} size="small" onClick={() => removeMarker(marker)}>Удалить</Button>
                            </Popup>
                        </Marker>
                    ))
                }
            </MapContainer>
        </>
    );
}