# Abp vnext cli tools

## Front-end generate typescript api (based on openapi)

Usage:

```cmd
dotnet tool install --global Powerfly.Abp.Cli
abpcli generate api -url "https://localhost:44330/swagger/v1/swagger.json"
```

/utils/http.ts

```js
import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios'

export enum ContentTypeEnum {
  //json
  JSON = 'application/json',
  //form-data qs
  FORM_URLENCODED = 'application/x-www-form-urlencoded',
  //form-data upload
  FORM_DATA = 'multipart/form-data'
}

export type RequestConfig = AxiosRequestConfig & {}

export class HttpRequest {
  instance: AxiosInstance

  public constructor(config: RequestConfig, callback?: (instance: AxiosInstance) => void) {
    this.instance = axios.create({
      baseURL: '/',
      headers: {
        'Content-Type': ContentTypeEnum.JSON,
        'X-Requested-With': 'XMLHttpRequest',
        ...config.headers
      },
      withCredentials: true,
      timeout: 10 * 1000,
      ...config
    })

    callback?.(this.instance)
  }

  public async request<T>(config: RequestConfig): Promise<AxiosResponse<T>> {
    const data = await this.instance.request({
      ...config
    })
    return data
  }

  public async get<T>(url: string, config: RequestConfig): Promise<T> {
    const { data } = await this.request<T>({
      ...config,
      url,
      method: 'GET'
    })
    return data
  }

  public async post<T>(url: string, config: RequestConfig): Promise<T> {
    const { data } = await this.request<T>({
      ...config,
      url,
      method: 'POST'
    })
    return data
  }

  public async put<T>(url: string, config: RequestConfig): Promise<T> {
    const { data } = await this.request<T>({
      ...config,
      url,
      method: 'PUT'
    })
    return data
  }

  public async patch<T>(url: string, config: RequestConfig): Promise<T> {
    const { data } = await this.request<T>({
      ...config,
      url,
      method: 'PATCH'
    })
    return data
  }

  public async delete<T>(url: string, config: RequestConfig): Promise<T> {
    const { data } = await this.request<T>({
      ...config,
      url,
      method: 'DELETE'
    })
    return data
  }
}

```

/api/index.ts

```js
import AbpApi from './AbpApi'

export const useAbpApi = () => {
  return new AbpApi({}, (instance) => {
    instance.interceptors.request.use(
      (request) => {
        return request
      },
      (error) => {
        return Promise.reject(error)
      }
    )

    instance.interceptors.response.use(
      (response) => {
        return response
      },
      (error) => {
        return Promise.reject(error)
      }
    )
  })
}
```
