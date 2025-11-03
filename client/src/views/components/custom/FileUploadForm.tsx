import React from "react";
import { MediaObject } from "@/features/media/types";
import FileUpload from "./FileUpload";

interface FileUploadFormProps {
	onSubmit: (...args: any) => any;
	setParentMediaList: (...args: any) => any;
}

// TODO: This component needs to be migrated from MUI to shadcn/ui
// Temporary stub to allow build to succeed
const FileUploadForm: React.FC<FileUploadFormProps> = ({
	onSubmit,
	setParentMediaList,
}) => {
	return (
		<div className="w-full">
			<FileUpload
				limit={5}
				multiple={true}
				name="files"
				setParentMediaList={setParentMediaList}
			/>
		</div>
	);
};

export default FileUploadForm;
