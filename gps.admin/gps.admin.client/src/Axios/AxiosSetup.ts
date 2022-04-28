import axios from "axios";


export const ApiUrl = "https://icosy.ru/api"

const axiosInstance = axios.create({
    baseURL: 'https://icosy.ru/api',
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true
});


axiosInstance.interceptors.request.use(function (config) {
    const refreshToken = localStorage.getItem('token');
    if (refreshToken) {
        config.headers = { ...config.headers, Authorization: `Bearer ${refreshToken}` };
    }

    return config;
})


axiosInstance.interceptors.response.use(
    function (response) {
        return response
    },
    function (error) {
        if (error.request.status !== 401) {
            return Promise.reject(error);
        }

        if (error.config.url == "/auth/refresh" && error.request.status == 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('userid');
            return Promise.reject(error);
        }

        // if(error.request.responseURL.includes("/auth/login")){
        //     return Promise.reject(error);
        // }

        const token = localStorage.getItem('token');
        if (token) {
            return axiosInstance
                .post('/auth/refresh')
                .then(response => {
                    localStorage.setItem('token', response.data.token);
                    localStorage.setItem('userid', response.data.user.id);
                    error.response.config.headers['Authorization'] = 'Bearer ' + response.data.token;
                    return axios(error.response.config);
                })
                .catch((data): any => {
                    console.log(data);
                    return data
                });
        }

        return Promise.reject(error);
    });

export default axiosInstance;