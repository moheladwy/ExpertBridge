import { AddCommentRequest } from "@/features/comments/types";
import { CreateJobPostingRequest } from "@/features/jobPostings/types";
import { useUploadMediaMutation } from "@/features/media/mediaSlice";
import { AddPostRequest } from "@/features/posts/types";
import { useEffect, useMemo } from "react";
import toast from "react-hot-toast";

const useCallbackOnMediaUploadSuccess = (
	callback: (...args: any) => any,
	request: AddPostRequest | AddCommentRequest | CreateJobPostingRequest
) => {
	const [uploadMedia, uploadResult] = useUploadMediaMutation();

	const memo = useMemo(() => request, [request]);

	useEffect(() => {
		if (uploadResult.isSuccess) {
			// Create a new object instead of mutating memo
			const updatedRequest = { ...memo, media: uploadResult.data };
			callback(updatedRequest);
		}
	}, [uploadResult.isSuccess, memo, callback]);

	useEffect(() => {
		if (uploadResult.isError) {
			toast.error("An error occurred while uploading media");
		}
	}, [uploadResult.isError]);

	return {
		uploadMedia,
		uploadResult,
	};
};

export default useCallbackOnMediaUploadSuccess;
