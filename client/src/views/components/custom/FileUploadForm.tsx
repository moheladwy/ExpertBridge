import { FormProvider, SubmitHandler, useForm } from "react-hook-form";
import { array, object, TypeOf, z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import FileUpload from "./FileUpload";
const imageUploadSchema = object({
	imageCover: z.instanceof(File),
	images: array(z.instanceof(File)),
});

type IUploadImage = TypeOf<typeof imageUploadSchema>;

interface FileUploadFormProps {
	onSubmit: (...args: any) => any;
	setParentMediaList: (...args: any) => any;
}

const FileUploadForm: React.FC<FileUploadFormProps> = ({
	setParentMediaList,
}) => {
	const methods = useForm<IUploadImage>({
		resolver: zodResolver(imageUploadSchema),
	});

	const onSubmitHandler: SubmitHandler<IUploadImage> = async (values) => {
		const formData = new FormData();
		formData.append("imageCover", values.imageCover);

		if (values.images.length > 0) {
			values.images.forEach((el) => formData.append("images", el));
		}
	};

	return (
		<FormProvider {...methods}>
			<form
				noValidate
				autoComplete="off"
				onSubmit={methods.handleSubmit(onSubmitHandler)}
				className="w-full"
			>
				<FileUpload
					limit={3}
					multiple
					name="images"
					setParentMediaList={setParentMediaList}
				/>
			</form>
		</FormProvider>
	);
};

export default FileUploadForm;
