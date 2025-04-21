import { AddCommentRequest } from "@/features/comments/types";
import { useUploadMediaMutation } from "@/features/media/mediaSlice";
import { AddPostRequest } from "@/features/posts/types";
import { useEffect, useMemo } from "react";
import toast from "react-hot-toast";

export default (callback: (...args: any) => any, request: AddPostRequest | AddCommentRequest) => {
  const [uploadMedia, uploadResult] = useUploadMediaMutation();

  const memo = useMemo(() => request, [request]);

  useEffect(() => {
    if (uploadResult.isSuccess) {
      memo.media = uploadResult.data;
      callback(request);
    }
  }, [uploadResult.isSuccess]);

  useEffect(() => {
    if (uploadResult.isError) {
      toast.error('An error occurred while uploading media');
    }
  }, [uploadResult.isError]);

  return {
    uploadMedia,
    uploadResult
  };
};


