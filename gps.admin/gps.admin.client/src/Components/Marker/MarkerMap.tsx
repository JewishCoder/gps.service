import { Button, ButtonGroup, makeStyles } from "@material-ui/core";
import { LatLngExpression, LatLngLiteral, LeafletMouseEvent } from "leaflet";
import { ReactElement, useState } from "react";
import { MapContainer, Marker, Popup, TileLayer, Tooltip } from "react-leaflet";
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
}

export function MarkerMap({ isEditMode, centerPosition, zoom }: IMarkerMapProps): ReactElement {
    const classes = useStyles();


    const [markers, setMarkers] = useState<IMarkerSate[]>([]);
    const [newMarkerDialogState, setNewMarkerDialogState] = useState(false);
    const [updateMarkerDialogState, setUpdateMarkerDialogState] = useState(false);
    const [newPosition, setNewPosition] = useState<LatLngExpression | null>(null);
    const [initDialogMarker, setinitDialogMarker] = useState<IMarkerSate | null>(null);

    const addMarker = (marker: IMarkerSate) => {
        setMarkers(markers => [...markers, marker]);
    }

    const removeMarker = (id: number) => {
        setMarkers(markers.filter(x => x.id != id));
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

    const onNewMarkerDialogSaveClick = (name: string) => {

        if (newPosition) {
            addMarker({ name, position: newPosition, id: markers.length + 1 });
            setNewPosition(null);
            setNewMarkerDialogState(false);
        }
        else {
            console.warn('marker position not found');
        }
    }

    const onUpdateMarkerDialogCancelClick = () => {
        setinitDialogMarker(null);
        setUpdateMarkerDialogState(false);
    }

    const onUpdateMarkerDialogSaveClick = (name: string) => {
        if (initDialogMarker == null) return;

        const updatedMarkes = markers.map((item) => {
            if (item.id == initDialogMarker.id) {
                return { id: initDialogMarker.id, name: name, position: initDialogMarker.position };
            }

            return item;
        });

        setMarkers(updatedMarkes);
        setinitDialogMarker(null);
        setUpdateMarkerDialogState(false);
    }

    return (
        <>
            <MarkerCreationModal isOpen={newMarkerDialogState} title={"Создание точки"}  markerName={""} onSaveClick={onNewMarkerDialogSaveClick} onCancelClick={onNewMarkerDialogCancelClick} />
            <MarkerCreationModal isOpen={updateMarkerDialogState} title={"Изменение точки"} markerName={initDialogMarker?.name ?? ""} onSaveClick={onUpdateMarkerDialogSaveClick} onCancelClick={onUpdateMarkerDialogCancelClick} />
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
                                <span>{marker.id} {marker.name}</span>
                            </Tooltip>
                            <Popup>
                                <Button disabled={!isEditMode} size="small" onClick={() => updateMarker(marker)}>Изменить</Button><br />
                                <Button disabled={!isEditMode} size="small" onClick={() => removeMarker(marker.id)}>Удалить</Button>
                            </Popup>
                        </Marker>
                    ))
                }
            </MapContainer>
        </>
    );
}