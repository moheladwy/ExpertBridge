import React from "react";
import { MediaObject } from "@/features/media/types";

interface FileUploadProps {
	limit: number;
	multiple: boolean;
	name: string;
	setParentMediaList: (...args: any) => any;
}

// TODO: This component needs to be migrated from MUI to shadcn/ui
// Temporary stub to allow build to succeed
const FileUpload: React.FC<FileUploadProps> = ({
	limit,
	multiple,
	name,
	setParentMediaList,
}) => {
	return (
		<div className="p-4 border-2 border-dashed border-gray-300 rounded-lg">
			<p className="text-gray-500">
				File upload component - Migration in progress
			</p>
			<input
				type="file"
				multiple={multiple}
				className="mt-2"
				onChange={(e) => {
					// Placeholder functionality
					console.log("Files selected:", e.target.files);
				}}
			/>
		</div>
	);
};

export default FileUpload;
