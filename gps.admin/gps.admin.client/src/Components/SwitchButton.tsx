import React from 'react';
import FormGroup from '@material-ui/core/FormGroup';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Switch from '@material-ui/core/Switch';

interface ISwitchButtonProps {
  classesName: string | undefined,
  label: string,
  onChange: (value: boolean) => void,
}

export default function SwitchButton({ label, onChange,classesName }: ISwitchButtonProps) {
  const [state, setState] = React.useState({
    checked: false,
  });

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setState({ ...state, [event.target.name]: event.target.checked });
    onChange(event.target.checked);
  };

  return (
    <FormControlLabel className={classesName}
      control={
        <Switch
          checked={state.checked}
          onChange={handleChange}
          name="checked"
          color="primary"
        />
      }
      label={label}
      value="start"
      labelPlacement="start"
    />
  );
}