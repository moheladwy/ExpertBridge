// import { emptyApiSlice as api } from "@/features/api/apiSlice";

// const injectedRtkApi = api.injectEndpoints({
//   endpoints: (build) => ({
//     postApiAuthLogin: build.mutation<
//       PostApiAuthLoginApiResponse,
//       PostApiAuthLoginApiArg
//     >({
//       query: (queryArg) => ({
//         url: `/api/Auth/login`,
//         method: "POST",
//         body: queryArg.loginRequest,
//       }),
//     }),
//     postApiAuthRegister: build.mutation<
//       PostApiAuthRegisterApiResponse,
//       PostApiAuthRegisterApiArg
//     >({
//       query: (queryArg) => ({
//         url: `/api/Auth/register`,
//         method: "POST",
//         body: queryArg.registerRequest,
//       }),
//     }),
//     getApiMediaDownloadByKey: build.query<
//       GetApiMediaDownloadByKeyApiResponse,
//       GetApiMediaDownloadByKeyApiArg
//     >({
//       query: (queryArg) => ({ url: `/api/Media/download/${queryArg.key}` }),
//     }),
//     getApiMediaUrlByKey: build.query<
//       GetApiMediaUrlByKeyApiResponse,
//       GetApiMediaUrlByKeyApiArg
//     >({
//       query: (queryArg) => ({ url: `/api/Media/url/${queryArg.key}` }),
//     }),
//     getApiMediaPresignedUrlByKey: build.query<
//       GetApiMediaPresignedUrlByKeyApiResponse,
//       GetApiMediaPresignedUrlByKeyApiArg
//     >({
//       query: (queryArg) => ({
//         url: `/api/Media/presigned-url/${queryArg.key}`,
//       }),
//     }),
//     postApiMediaUpload: build.mutation<
//       PostApiMediaUploadApiResponse,
//       PostApiMediaUploadApiArg
//     >({
//       query: (queryArg) => ({
//         url: `/api/Media/upload`,
//         method: "POST",
//         body: queryArg.body,
//       }),
//     }),
//     deleteApiMediaDeleteByKey: build.mutation<
//       DeleteApiMediaDeleteByKeyApiResponse,
//       DeleteApiMediaDeleteByKeyApiArg
//     >({
//       query: (queryArg) => ({
//         url: `/api/Media/delete/${queryArg.key}`,
//         method: "DELETE",
//       }),
//     }),
//     getApiUserGetByFirebaseId: build.query<
//       GetApiUserGetByFirebaseIdApiResponse,
//       GetApiUserGetByFirebaseIdApiArg
//     >({
//       query: (queryArg) => ({ url: `/api/User/get/${queryArg.firebaseId}` }),
//     }),
//     postApiUserRegister: build.mutation<
//       PostApiUserRegisterApiResponse,
//       PostApiUserRegisterApiArg
//     >({
//       query: (queryArg) => ({
//         url: `/api/User/register`,
//         method: "POST",
//         body: queryArg.registerUserRequest,
//       }),
//     }),
//   }),
//   overrideExisting: false,
// });
// export { injectedRtkApi as appApiSlice };
// export type PostApiAuthLoginApiResponse = unknown;
// export type PostApiAuthLoginApiArg = {
//   loginRequest: LoginRequest;
// };
// export type PostApiAuthRegisterApiResponse = unknown;
// export type PostApiAuthRegisterApiArg = {
//   registerRequest: RegisterRequest;
// };
// export type GetApiMediaDownloadByKeyApiResponse = unknown;
// export type GetApiMediaDownloadByKeyApiArg = {
//   key: string;
// };
// export type GetApiMediaUrlByKeyApiResponse = unknown;
// export type GetApiMediaUrlByKeyApiArg = {
//   key: string;
// };
// export type GetApiMediaPresignedUrlByKeyApiResponse = unknown;
// export type GetApiMediaPresignedUrlByKeyApiArg = {
//   key: string;
// };
// export type PostApiMediaUploadApiResponse = unknown;
// export type PostApiMediaUploadApiArg = {
//   body: {
//     file?: Blob;
//   };
// };
// export type DeleteApiMediaDeleteByKeyApiResponse = unknown;
// export type DeleteApiMediaDeleteByKeyApiArg = {
//   key: string;
// };
// export type GetApiUserGetByFirebaseIdApiResponse =
//   /** status 200 OK */ UserResponse;
// export type GetApiUserGetByFirebaseIdApiArg = {
//   firebaseId: string;
// };
// export type PostApiUserRegisterApiResponse = /** status 200 OK */ UserResponse;
// export type PostApiUserRegisterApiArg = {
//   registerUserRequest: RegisterUserRequest;
// };
// export type LoginRequest = {
//   email: string | null;
//   password: string | null;
//   twoFactorCode?: string | null;
//   twoFactorRecoveryCode?: string | null;
// };
// export type RegisterRequest = {
//   email: string | null;
//   password: string | null;
// };
// export type UserResponse = {
//   id?: string | null;
//   firebaseId?: string | null;
//   email?: string | null;
//   username?: string | null;
//   firstName?: string | null;
//   lastName?: string | null;
//   isBanned?: boolean;
//   isDeleted?: boolean;
// };

// export type RegisterUserRequest = {
//   firebaseId?: string | null;
//   email?: string | null;
//   username?: string | null;
//   firstName?: string | null;
//   lastName?: string | null;
// };

// export const {
//   usePostApiAuthLoginMutation,
//   usePostApiAuthRegisterMutation,
//   useGetApiMediaDownloadByKeyQuery,
//   useGetApiMediaUrlByKeyQuery,
//   useGetApiMediaPresignedUrlByKeyQuery,
//   usePostApiMediaUploadMutation,
//   useDeleteApiMediaDeleteByKeyMutation,
//   useGetApiUserGetByFirebaseIdQuery,
//   usePostApiUserRegisterMutation,
// } = injectedRtkApi;
