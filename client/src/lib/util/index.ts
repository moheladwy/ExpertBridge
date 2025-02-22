export { default as useLoadingValue } from './useLoadingValue';

export type LoadingHook<T, E> = [T | undefined, boolean, E | undefined];