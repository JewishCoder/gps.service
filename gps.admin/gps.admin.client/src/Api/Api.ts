import { LatLng } from 'leaflet';
import {HubConnectionState, HubConnectionBuilder, HubConnection} from "@microsoft/signalr";
import axiosInstance, { ApiUrl } from '../Axios/AxiosSetup';
import { IUser } from '../Components/UsersTable/UsersTable';

export interface IPoint {
    id: number,
    name: string,
    latitude: number,
    longitude: number
}

export class Api {

    static loginUser(login: string, password: string) {
        return axiosInstance.post("/auth/login", {
            login,
            password,
        }).then((res): any => {
            if(res.data.token && res.data.user){
                localStorage.setItem('token', res.data.token);
                localStorage.setItem('userid', res.data.user.id);
            }
            return res.data
        });
    }

    static refreshLogin() {
        return axiosInstance.post("/auth/refresh")
            .then((res): any => res.data);
    }

    static getUsers() {
        return axiosInstance.get("/users")
            .then((res): any => res.data);
    }

    static createUser(user: IUser, password: string) {
        console.log({
            login: user.login,
            name: user.name,
            role: user.role,
            password: password,
        });
        return axiosInstance.post("/users", {
            login: user.login,
            name: user.name,
            role: user.role,
            password: password,
        }).then((res): any => res.data);
    }

    static updateUser(user: IUser, password: string | null) {
        return axiosInstance.put("/users/" + user.id, {
            login: user.login,
            name: user.name,
            role: user.role,
            password: password,
        }).then((res): any => res.data);
    }

    static deleteUser(id: number) {
        return axiosInstance.delete("/users/" + id)
            .then((res): boolean => res.data);
    }

    static getPoints() {
        return axiosInstance.get("/points").then((res): IPoint[] => res.data);
    }

    static createPoint(name: string, position: LatLng) {
        return axiosInstance.post("/points", {
            name: name,
            latitude: position.lat,
            longitude: position.lng,
        }).then((res): IPoint => res.data);
    }

    static updatePoint(id: number, name: string, position: LatLng) {
        return axiosInstance.put("/points/" + id, {
            name: name,
            latitude: position.lat,
            longitude: position.lng,
        }).then((res): IPoint => res.data);
    }

    static deletePoint(id: number) {
        return axiosInstance.delete("/points/" + id)
            .then((res): boolean => res.data);
    }

    static getNotifyService() : HubConnection | null{
        const refreshToken = localStorage.getItem('token');
        if(refreshToken){
            const nConnection = new HubConnectionBuilder()
            .withUrl(ApiUrl + "/notifications", {
                accessTokenFactory: () => refreshToken
            })
            .withAutomaticReconnect()
            .build();

            return nConnection;
        }

        return null;
    }

    static getUserId(): number {
        const userId = localStorage.getItem('userid');
        if(userId){
            return Number.parseInt(userId);
        }

        return 0;
    }
}